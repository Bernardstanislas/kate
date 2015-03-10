using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Kate.Bots.Workers;
using Kate.Commands;
using Kate.IO;
using Kate.Maps;
using Kate.Types;

namespace Kate.Bots
{
    public abstract class TurnByTurnBot : Bot
    {
        public int TreeTimeout { get; private set; }
        public int ChoiceTimeout { get; private set; }
        public Worker Worker { get; private set; }
        public Dictionary<int, TreeNode> Tree { get; set; }

        public TurnByTurnBot(IClient socket, string name, Worker worker, int treeTimeout, int choiceTimeout) : base(socket, name) 
        {
            Worker = worker;
            TreeTimeout = treeTimeout;
            ChoiceTimeout = choiceTimeout;
        }

        protected abstract ICollection<Move> selectBestNode(int depth);

        #region playTurn implementation
        protected override ICollection<Move> playTurn()
        {
            var elapsedTime = new Stopwatch();
            elapsedTime.Start();

            var turn = Owner.Me;

            Tree = new Dictionary<int, TreeNode> 
            { 
                {map.GetHashCode(), new TreeNode(map)} 
            };

            // A List is better than an Array for creating the Task pool because it's not slower and easier to write
            var taskPool = new List<Task<Tuple<List<TreeNode>, int>>>
            {
                Task.Factory.StartNew(() => createWorker(map, turn))
            };

            int depth = 0;
            while (elapsedTime.ElapsedMilliseconds < TreeTimeout) 
            {
                // But an Array is needed for Task.WaitAll()
                var taskPoolArray = taskPool.ToArray();

                // And it allows us to clear the initial list right now to fill it again with the new Tasks.
                taskPool.Clear();
 
                if (Task.WaitAll(taskPoolArray, TreeTimeout - (int)elapsedTime.ElapsedMilliseconds))
                {
                    depth++;
                    turn = turn.Opposite();

                    for (int i = 0; i < taskPoolArray.Length; i++)
                    {
                        var nodeArray = taskPoolArray[i].Result.Item1.ToArray();
                        var parentHash = taskPoolArray[i].Result.Item2;

                        for (int j = 0; j < nodeArray.Length; i++)
                        {
                            Tree.Add(nodeArray[j].GetHashCode(), nodeArray[j]); // Add new node to Tree
                            Tree[parentHash].AddChildren(nodeArray[j].GetHashCode()); // Add new node to parent node children

                            // Start new Task for this new node
                            taskPool.Add(Task.Factory.StartNew(() => createWorker(nodeArray[j].Map, turn)));
                        }
                    }
                }
            }
            elapsedTime.Stop();

            return selectBestNode(depth);
        }

        private Tuple<List<TreeNode>, int> createWorker(IMap map, Owner turn)
        {
            var worker = WorkerFactory.Build(Worker, map, turn);
            return Tuple.Create(worker.ComputeNodeChildren(), map.GetHashCode());
        }
        #endregion
    }
}
