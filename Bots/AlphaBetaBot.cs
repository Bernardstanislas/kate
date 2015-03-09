using System;
using System.Collections.Generic;

using Kate.Bots.Workers;
using Kate.Commands;
using Kate.IO;
using Kate.Types;

namespace Kate.Bots
{
    class AlphaBetaBot : TurnByTurnBot
    {
        public AlphaBetaBot(SocketClient socket, string name, int treeTimeout, int choiceTimeout) 
            : base(socket, name, Worker.DefaultWorker, treeTimeout, choiceTimeout) { }
        
        protected override ICollection<Move> selectBestNode(int depth)
        {
            var nodeHashes = Tree[map.GetHashCode()].ChildrenHashes;
            var bestNodeHash = nodeHashes[0];
            float bestHeuristic = 0;

            foreach (var nodeHash in nodeHashes)
            {
                var node = Tree[nodeHash];
                var heuristic = iterate(node, depth, float.MinValue, float.MaxValue, Owner.Me);
                if (heuristic > bestHeuristic)
                {
                    bestHeuristic = heuristic;
                    bestNodeHash = nodeHash;
                }
            }
            return Tree[bestNodeHash].MoveList;
        }

        private float iterate(TreeNode node, int depth, float alpha, float beta, Owner player)
        {
            if (depth == 1 || node.Map.HasPlayerWon(player))
                return node.Heuristic(player);

            if (player == Owner.Me)
            {
                foreach (var childHash in node.ChildrenHashes)
                {
                    var child = Tree[childHash];

                    alpha = Math.Max(alpha, iterate(child, depth - 1, alpha, beta, Owner.Opponent));
                    if (beta < alpha)
                        break;
                }
                return alpha;
            }
            else
            {
                foreach (var childHash in node.ChildrenHashes)
                {
                    var child = Tree[childHash];

                    beta = Math.Min(beta, iterate(child, depth - 1, alpha, beta, Owner.Me));
                    if (beta < alpha)
                        break;
                }
                return beta;
            }
        }
    }
}
