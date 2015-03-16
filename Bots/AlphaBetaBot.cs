using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Kate.Bots.Workers;
using Kate.Commands;
using Kate.Heuristics;
using Kate.IO;
using Kate.Maps;
using Kate.Types;

namespace Kate.Bots
{
    public class AlphaBetaBot : Bot
    {
        protected readonly int timeout;
        protected readonly Worker worker;
        protected Dictionary<int, TreeNode> tree;

        public AlphaBetaBot(IClient socket, string name, Worker worker, int timeout) : base(socket, name) 
        {
            this.worker = worker;
            this.timeout = timeout;
        }

        protected override ICollection<Move> playTurn()
        {
            var elapsedTime = new Stopwatch();
            elapsedTime.Start();
            tree = new Dictionary<int, TreeNode>();
            tree.Add(map.GetHashCode(), new TreeNode(map));

            var childNodes = getChildNodes(map, Owner.Me);
            var bestNode = tree[map.GetHashCode()];

            int depth = 0;
            while (elapsedTime.ElapsedMilliseconds < timeout)
            {
                depth += 2;
                var task = new Task(() =>
                {
                    var alpha = float.MinValue;
                    foreach (var child in childNodes)
                    {
                        var newAlpha = alphaBeta(child, depth - 1, alpha, float.MaxValue, Owner.Opponent);
                        if (newAlpha > alpha)
                        {
                            alpha = newAlpha;
                            bestNode = child;
                        }
                    }
                });
                task.Start();
                task.Wait((int)Math.Max(0, timeout - elapsedTime.ElapsedMilliseconds));
            }

            Console.Write("Depth computed: ");
            Console.WriteLine(depth);

            elapsedTime.Stop();

            return bestNode.MoveList;
        }

        private float alphaBeta(TreeNode node, int depth, float alpha, float beta, Owner player)
        {
            if (depth == 0 || node.Map.HasGameEnded())
                return node.Heuristic;

            if (player == Owner.Me)
            {
                var childNodes = getChildNodes(node.Map, Owner.Me);
                foreach (var child in childNodes)
                {
                    alpha = Math.Max(alpha, alphaBeta(child, depth - 1, alpha, beta, Owner.Opponent));
                    if (beta < alpha)
                        break;
                }
                return alpha;
            }
            else
            {
                var childNodes = getChildNodes(node.Map, Owner.Opponent);
                foreach (var child in childNodes)
                {
                    beta = Math.Min(beta, alphaBeta(child, depth - 1, alpha, beta, Owner.Me));
                    if (beta < alpha)
                        break;
                }
                return beta;
            }
        }

        private IEnumerable<TreeNode> getChildNodes(IMap map, Owner turn)
        {
            var parentHash = map.GetHashCode();
            var parentChildrenHashes = tree[parentHash].GetChildrenHashes(turn);

            if (parentChildrenHashes.Count() > 0)
                return parentChildrenHashes.Select(hash => tree[hash]);
            
            var newWorker = WorkerFactory.Build(worker, map, turn);

            var nodeChildren = newWorker.ComputeNodeChildren();

            foreach (var childNode in nodeChildren)
                if(!tree.ContainsKey(childNode.GetHashCode()))
                    tree.Add(childNode.GetHashCode(), childNode);

            tree[parentHash].AddChildren(nodeChildren.Select(node => node.GetHashCode()), turn);
            
            return nodeChildren;
        }
    }
}
