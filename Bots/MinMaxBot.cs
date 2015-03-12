using System;
using System.Collections.Concurrent;
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
    public abstract class MinMaxBot : Bot
    {
        protected readonly int timeout;
        protected readonly Worker worker;
        protected Dictionary<int, TreeNode> tree;

        public MinMaxBot(IClient socket, string name, Worker worker, int timeout) : base(socket, name) 
        {
            this.worker = worker;
            this.timeout = timeout;
        }

        protected abstract float browseTree(TreeNode node, int depth, float alpha, float beta, Owner player);

        protected override ICollection<Move> playTurn()
        {
            var elapsedTime = new Stopwatch();
            elapsedTime.Start();

            tree = new Dictionary<int, TreeNode> 
            { 
                {map.GetHashCode(), new TreeNode(map, HeuristicManager.Instance.GetScore)} 
            };

            var childNodes = getChildNodes(map, Owner.Me);
            
            float bestHeuristic = float.MaxValue;
            var bestNode = tree[map.GetHashCode()];

            var nodes = new ConcurrentBag<TreeNode>(childNodes);
            Parallel.ForEach(nodes, node =>
            {
                var heuristic = browseTree(node, 2, float.MinValue, float.MaxValue, Owner.Opponent);
                if (heuristic < bestHeuristic)
                {
                    bestHeuristic = heuristic;
                    bestNode = node;
                }
            });

            Console.WriteLine(elapsedTime.ElapsedMilliseconds);
            elapsedTime.Stop();

            return bestNode.MoveList;
        }        

        protected IEnumerable<TreeNode> getChildNodes(IMap map, Owner turn)
        {
            var parentNode = tree[map.GetHashCode()];

            if (parentNode.ChildrenHashes.Count > 0)
                return parentNode.ChildrenHashes.Select(hash => tree[hash]);
            
            var newWorker = WorkerFactory.Build(worker, map, turn);
            return newWorker.ComputeNodeChildren().Select(childNode =>
            {
                var childKey = childNode.GetHashCode();
                parentNode.AddChildren(childKey);

                if (!tree.ContainsKey(childKey))
                    tree.Add(childKey, childNode);

                return childNode;
            });
        }
    }
}
