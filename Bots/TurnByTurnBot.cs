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
    public abstract class TurnByTurnBot : AbstractBot
    {
        protected Stopwatch elapsedTime;

        public int TreeTimeout { get; private set; }
        public int ChoiceTimeout { get; private set; }
        public Worker Worker { get; private set; }
        public Dictionary<int, TreeNode> Tree { get; set; }

        public TurnByTurnBot(SocketClient socket, string name, Worker worker, int treeTimeout, int choiceTimeout) : base(socket, name) 
        {
            Worker = worker;
            TreeTimeout = treeTimeout;
            ChoiceTimeout = choiceTimeout;
        }

        public override ICollection<Move> playTurn()
        {
            elapsedTime.Start();

            var turn = Owner.Me;

            Tree = new Dictionary<int, TreeNode> 
            { 
                {map.GetHashCode(), new TreeNode(map, new float[2] {0,0})} 
            };

            // A List is better than an Array for creating the Task pool because it's not slower and easier to write
            var taskPool = new List<Task<Tuple<List<TreeNode>, int>>>
            {
                Task.Factory.StartNew(() => CreateWorker(map, turn))
            };

            while (elapsedTime.ElapsedMilliseconds < TreeTimeout) 
            {
                // But an Array is needed for Task.WaitAll()
                var taskPoolArray = taskPool.ToArray();

                // And it allows us to clear the initial list right now to fill it again with the new Tasks.
                taskPool.Clear();
 
                if (Task.WaitAll(taskPoolArray, TreeTimeout - (int)elapsedTime.ElapsedMilliseconds))
                {
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
                            taskPool.Add(Task.Factory.StartNew(() => CreateWorker(nodeArray[j].Map, turn)));
                        }
                    }
                }
            }
            elapsedTime.Stop();

            return SelectBestNode();
        }

        private Tuple<List<TreeNode>, int> CreateWorker(IMap map, Owner turn)
        {
            var worker = WorkerFactory.Build(Worker, map, turn);
            return Tuple.Create(worker.ComputeNodeChildren(), map.GetHashCode());
        }

        protected abstract ICollection<Move> SelectBestNode();
    }
}
