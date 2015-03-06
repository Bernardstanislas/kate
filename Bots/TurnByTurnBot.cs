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
        public Dictionary<int, TreeNode<IMap>> Tree { get; set; }

        public TurnByTurnBot(SocketClient socket, string name, Worker worker, int timeout) : base(socket, name) 
        {
            Worker = worker;
            Timeout = timeout;
        }
        
        // Put the result of a Worker at its right place in the tree
        public void AddWorkerResult(List<TreeNode<IMap>> nodes, int parentHash)
        {
            foreach (var node in nodes)
            {
                Tree.Add(node.GetHashCode(), node);
                Tree[parentHash].AddChildren(node.GetHashCode());
            }
        }

        public override ICollection<Move> playTurn()
        {
            elapsedTime.Start();

            var turn = Owner.Me;

            Tree = new Dictionary<int, TreeNode<IMap>> 
            { 
                {map.GetHashCode(), new TreeNode<IMap>(map, 0)} 
            };

            var taskPool = new List<Task<Tuple<List<TreeNode<IMap>>, int>>>
            {
                Task.Factory.StartNew(() => CreateWorker(map, turn))
            };

            while (elapsedTime.ElapsedMilliseconds < Timeout) 
            {
                if (Task.WaitAll(taskPool.ToArray(), Timeout - (int)elapsedTime.ElapsedMilliseconds))
                {
                    var results = new List<Tuple<List<TreeNode<IMap>>, int>>();

                    for (int i = 0; i < taskPool.Count; i++)
                        results[i] = taskPool[i].Result;

                    taskPool.Clear();

                    turn = turn == Owner.Me ? Owner.Opponent : Owner.Me;

                    for (int i = 0; i < results.Count; i++)
                    {
                        var nodeList = results[i].Item1;
                        var parentHash = results[i].Item2;

                        AddWorkerResult(nodeList, parentHash);

                        for (int j = 0; j < nodeList.Count; j++)
                            taskPool.Add(Task.Factory.StartNew(() => CreateWorker(nodeList[j].Value, turn)));
                    }
                }
            }
            elapsedTime.Stop();

            return GetReturnNode();
        }

        private Tuple<List<TreeNode<IMap>>, int> CreateWorker(IMap map, Owner turn)
        {
            var worker = WorkerFactory.Build(Worker, map, turn);
            return Tuple.Create(worker.ComputeNodeChildren(), map.GetHashCode());
        }

        protected abstract ICollection<Move> GetReturnNode();
    }
}
