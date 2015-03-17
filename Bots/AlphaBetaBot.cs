﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

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
        protected Dictionary<int, TreeNode> tree;

        public AlphaBetaBot(IClient socket, string name, int timeout) : base(socket, name) 
        {
            this.timeout = timeout;
        }

        protected override ICollection<Move> playTurn()
        {
            var elapsedTime = new Stopwatch();
            elapsedTime.Start();
            tree = new Dictionary<int, TreeNode>();
            tree.Add(map.GetHashCode(), new TreeNode(map));

            var childNodesWithMoveList = getChildNodeHashesWithMoveList(map.GetHashCode(), Owner.Me);

            // If we don't have the time to compute anything, we'll return a random moveList.
            var random = new Random();
            var CNWMLList = childNodesWithMoveList.ToList();

            var bestMoveList = new List<Move>();
            while(bestMoveList.Count == 0)
                bestMoveList = CNWMLList[random.Next(CNWMLList.Count)].Item2;

            int depth = 0;
            while (elapsedTime.ElapsedMilliseconds < timeout)
            {
                depth += 2;
                var task = new Task(() =>
                {
                    var alpha = float.MinValue;
                    foreach (var child in childNodesWithMoveList)
                    {
                        var newAlpha = alphaBeta(child.Item1, depth - 1, alpha, float.MaxValue, Owner.Opponent);
                        if (newAlpha > alpha)
                        {
                            alpha = newAlpha;
                            if(child.Item2.Count != 0)
                                bestMoveList = child.Item2;
                        }
                    }
                });
                task.Start();
                task.Wait((int)Math.Max(0, timeout - elapsedTime.ElapsedMilliseconds));
            }

            Console.Write("Depth computed: ");
            Console.WriteLine(depth);
            Console.WriteLine("Number of moves : {0}", bestMoveList.Count);
            elapsedTime.Stop();

            return bestMoveList;
        }

        private float alphaBeta(int nodeHash, int depth, float alpha, float beta, Owner player)
        {
            if (tree.ContainsKey(nodeHash))
            {
                var node = tree[nodeHash];
                if (depth == 0 || node.Map.HasGameEnded())
                    return node.Heuristic;
            }

            else
                return 0;

            if (player == Owner.Me)
            {
                var childNodeHashes = getChildNodeHashes(nodeHash, Owner.Me);
                foreach (var child in childNodeHashes)
                {
                    alpha = Math.Max(alpha, alphaBeta(child, depth - 1, alpha, beta, Owner.Opponent));
                    if (beta < alpha)
                        break;
                }
                return alpha;
            }
            else
            {
                var childNodeHashes = getChildNodeHashes(nodeHash, Owner.Opponent);
                foreach (var child in childNodeHashes)
                {
                    beta = Math.Min(beta, alphaBeta(child, depth - 1, alpha, beta, Owner.Me));
                    if (beta < alpha)
                        break;
                }
                return beta;
            }
        }

        private IEnumerable<int> getChildNodeHashes(int nodeHash, Owner turn)
        {
            var node = tree[nodeHash];
            var existingNodeChildrenHashes = node.GetChildrenHashes(turn);

            if (existingNodeChildrenHashes.Count() > 0)
                return existingNodeChildrenHashes;

            var nodeChildren = NodeComputer.GetChildren(node.Map, turn);

            foreach (var childNode in nodeChildren)
            {
                var key = childNode.GetHashCode();
                if (!tree.ContainsKey(key))
                    tree.Add(key, childNode);
            }

            var nodeChildrenHashes = nodeChildren.Select(child => child.GetHashCode());
            node.AddChildren(nodeChildrenHashes, turn);

            return nodeChildrenHashes;
        }

        private IEnumerable<Tuple<int, List<Move>>> getChildNodeHashesWithMoveList(int nodeHash, Owner turn)
        {
            var node = tree[nodeHash];
            var nodeChildren = NodeComputer.GetChildrenWithMoveList(node.Map, turn);

            foreach (var childNode in nodeChildren)
            {
                var key = childNode.Item1.GetHashCode();
                if (!tree.ContainsKey(key))
                    tree.Add(key, childNode.Item1);
            }

            if (node.GetChildrenHashes(turn).Count() == 0)
            {
                var nodeChildrenHashes = nodeChildren.Select(child => child.Item1.GetHashCode());
                node.AddChildren(nodeChildrenHashes, turn);
            }

            return nodeChildren.Select(child => Tuple.Create(child.Item1.GetHashCode(), child.Item2));
        }
    }
}
