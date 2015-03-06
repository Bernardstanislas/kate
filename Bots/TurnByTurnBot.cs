using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
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

        public int Timeout { get; private set; }
        public Worker Worker { get; private set; }
        public TreeNode<IMap> Tree { get; set; }

        public TurnByTurnBot(SocketClient socket, string name, Worker worker, int timeout) : base(socket, name) 
        {
            Worker = worker;
            Timeout = timeout;
        }
        
        // Put the result of a Worker at its right place in the tree
        public void AddWorkerResult(List<TreeNode<IMap>> nodes, int[] position)
        {
            var currentNode = Tree;

            if (position[0] != -1)
                foreach (int depth in position)
                    currentNode = currentNode.Children[depth];

            currentNode.Children = nodes;
        }

        public override ICollection<Move> playTurn()
        {
            elapsedTime.Start();

            Tree = new TreeNode<IMap>(map);

            var taskPool = new List<Task<Tuple<List<TreeNode<IMap>>, int[]>>>
            {
                Task.Factory.StartNew(() => CreateWorker(Tree.Value, Owner.Me, new int[1] { -1 }))
            };

            while (elapsedTime.ElapsedMilliseconds < Timeout) 
            {
                if (Task.WaitAll(taskPool.ToArray(), Timeout - (int)elapsedTime.ElapsedMilliseconds))
                {
                    var turn = taskPool[0].Result.Item2.Length % 2 == 0 ? Owner.Opponent : Owner.Me;

                    var results = new List<Tuple<List<TreeNode<IMap>>, int[]>>();

                    for (int i = 0; i < taskPool.Count; i++)
                        results[i] = taskPool[i].Result;

                    taskPool.Clear();

                    for (int i = 0; i < results.Count; i++)
                    {
                        var nodeList = results[i].Item1;
                        var position = results[i].Item2;

                        AddWorkerResult(nodeList, position);

                        for (int j = 0; j < results[i].Item1.Count; j++)
                        {
                            var newPosition = new int[position.Length + 1];
                            position.CopyTo(newPosition, 0);
                            newPosition[position.Length] = j;
                            taskPool.Add(Task.Factory.StartNew(() => CreateWorker(nodeList[j].Value, turn, newPosition)));
                        }
                    }
                }
            }

            elapsedTime.Stop();

            return GetReturnNode();
        }

        private Tuple<List<TreeNode<IMap>>, int[]> CreateWorker(IMap map, Owner turn, int[] position)
        {
            var worker = WorkerFactory.Build(Worker, map, turn);
            return Tuple.Create(worker.computeNodeChildren(), position);
        }

        protected abstract ICollection<Move> GetReturnNode();
    }
}
