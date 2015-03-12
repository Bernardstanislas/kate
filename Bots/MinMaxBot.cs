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
        protected ConcurrentDictionary<int, TreeNode> tree;

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
            tree = new ConcurrentDictionary<int, TreeNode>();
            tree.GetOrAdd(map.GetHashCode(), new TreeNode(map, HeuristicManager.Instance.GetScore));

            var childNodes = getChildNodes(map, Owner.Me);

            float bestHeuristic = float.MaxValue;
            var bestNode = tree[map.GetHashCode()];

            int depth = 0;
            while(elapsedTime.ElapsedMilliseconds < timeout)
            {
                depth += 2;
                var task = new Task(() =>
                {
                    Parallel.ForEach(childNodes, node =>
                    {
                        var heuristic = browseTree(node, depth - 1, float.MinValue, float.MaxValue, Owner.Opponent);
                        if (heuristic < bestHeuristic)
                        {
                            bestHeuristic = heuristic;
                            bestNode = node;
                        }
                    });
                });
                task.Start();
                task.Wait((int)Math.Max(0, timeout - elapsedTime.ElapsedMilliseconds));
            }

            Console.Write("Depth computed: ");
            Console.WriteLine(depth);
            elapsedTime.Stop();

            return bestNode.MoveList;
        }

        protected IEnumerable<TreeNode> getChildNodes(IMap map, Owner turn)
        {
            var parentHash = map.GetHashCode();
            var parentChildrenHashes = tree[parentHash].ChildrenHashes;

            if (parentChildrenHashes.ToList().Count > 0)
                return parentChildrenHashes.Select(hash => tree[hash]);
            
            var newWorker = WorkerFactory.Build(worker, map, turn);

            var nodeChildren = newWorker.ComputeNodeChildren();

            foreach (var childNode in nodeChildren)
                tree.GetOrAdd(childNode.GetHashCode(), childNode);

            var childNodeHashes = nodeChildren.Select(node => node.GetHashCode());

            var parentNode = tree[parentHash];
            tree.TryUpdate(parentHash, new TreeNode(map, HeuristicManager.Instance.GetScore, parentNode.MoveList, childNodeHashes), parentNode);
            
            return nodeChildren;
        }
    }
}
