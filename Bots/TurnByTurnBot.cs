using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Kate.Bots.Workers;
using Kate.Commands;
using Kate.Heuristic;
using Kate.IO;
using Kate.Maps;
using Kate.Types;

namespace Kate.Bots
{
    public abstract class TurnByTurnBot : Bot
    {
        protected readonly int treeTimeout;
        protected readonly int choiceTimeout;
        protected readonly Worker worker;
        protected Dictionary<int, TreeNode> tree;

        public TurnByTurnBot(IClient socket, string name, Worker worker, int treeTimeout, int choiceTimeout) : base(socket, name) 
        {
            this.worker = worker;
            this.treeTimeout = treeTimeout;
            this.choiceTimeout = choiceTimeout;
        }

        protected abstract ICollection<Move> selectBestNode(int depth);

        #region playTurn implementation
        protected override ICollection<Move> playTurn()
        {
            var elapsedTime = new Stopwatch();
            elapsedTime.Start();

            var turn = Owner.Me;

            tree = new Dictionary<int, TreeNode> 
            { 
                {map.GetHashCode(), new TreeNode(map, HeuristicManager.Instance.GetScore)} 
            };

            // A List is better than an Array for creating the Task pool because it's not slower and easier to write
            var taskPool = new List<Task<Tuple<List<TreeNode>, int>>>
            {
                new Task<Tuple<List<TreeNode>, int>>(() => createWorker(map, turn))
            };

            int depth = 0;

            while (elapsedTime.ElapsedMilliseconds < treeTimeout) 
            {
                // But an Array is needed for Task.WaitAll()
                var taskPoolArray = taskPool.ToArray();

                // And it allows us to clear the initial list right now to fill it again with the new Tasks.
                taskPool.Clear();

                for (int i = 0; i < taskPoolArray.Length; i++)
                    taskPoolArray[i].Start();

                if (Task.WaitAll(taskPoolArray, Math.Max(0, treeTimeout - (int)elapsedTime.ElapsedMilliseconds)))
                {
                    depth++;
                    turn = turn.Opposite();

                    for (int i = 0; i < taskPoolArray.Length; i++)
                    {
                        var nodeArray = taskPoolArray[i].Result.Item1.ToArray();
                        var parentHash = taskPoolArray[i].Result.Item2;

                        for (int j = 0; j < nodeArray.Length; j++)
                        {
                            var value = nodeArray[j];
                            var key = value.GetHashCode();

                            if (!tree.TryGetValue(key, out value))
                                tree.Add(key, value); // Add new node to Tree
                            tree[parentHash].AddChildren(key); // Add new node to parent node children

                            // Start new Task for this new node
                            taskPool.Add(new Task<Tuple<List<TreeNode>, int>>(() => createWorker(value.Map, turn)));
                        }
                    }
                }
            }
            elapsedTime.Stop();

            return selectBestNode(depth);
        }

        private Tuple<List<TreeNode>, int> createWorker(IMap map, Owner turn)
        {
            var workerTask = WorkerFactory.Build(worker, map, turn);
            return Tuple.Create(workerTask.ComputeNodeChildren().ToList(), map.GetHashCode());
        }
        #endregion
    }
}
