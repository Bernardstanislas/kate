﻿using System;
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

        public override ICollection<Move> playTurn()
        {
            elapsedTime.Start();

            var turn = Owner.Me;

            Tree = new Dictionary<int, TreeNode<IMap>> 
            { 
                {map.GetHashCode(), new TreeNode<IMap>(map, 0)} 
            };

            // A List is better than an Array for creating the Task pool because it's not slower and easier to write
            var taskPool = new List<Task<Tuple<List<TreeNode<IMap>>, int>>>
            {
                Task.Factory.StartNew(() => CreateWorker(map, turn))
            };

            while (elapsedTime.ElapsedMilliseconds < Timeout) 
            {
                // But an Array is needed for Task.WaitAll()
                var taskPoolArray = taskPool.ToArray();

                // And it allows us to clear the initial list right now to fill it again with the new Tasks.
                taskPool.Clear();
 
                if (Task.WaitAll(taskPoolArray, Timeout - (int)elapsedTime.ElapsedMilliseconds))
                {
                    turn = turn == Owner.Me ? Owner.Opponent : Owner.Me;

                    for (int i = 0; i < taskPoolArray.Length; i++)
                    {
                        var nodeArray = taskPoolArray[i].Result.Item1.ToArray();
                        var parentHash = taskPoolArray[i].Result.Item2;

                        for (int j = 0; j < nodeArray.Length; i++)
                        {
                            Tree.Add(nodeArray[j].GetHashCode(), nodeArray[j]); // Add new node to Tree
                            Tree[parentHash].AddChildren(nodeArray[j].GetHashCode()); // Add new node to parent node children

                            // Start new Task for this new node
                            taskPool.Add(Task.Factory.StartNew(() => CreateWorker(nodeArray[j].Value, turn)));
                        }
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
