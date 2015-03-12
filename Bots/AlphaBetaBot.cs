using System;

using Kate.Bots.Workers;
using Kate.IO;
using Kate.Types;

namespace Kate.Bots
{
    class AlphaBetaBot : MinMaxBot
    {
        public AlphaBetaBot(IClient socket, string name, int timeout)
            : base(socket, name, Worker.DefaultWorker, timeout) { }

        protected override float browseTree(TreeNode node, int depth, float alpha, float beta, Owner player)
        {
            if (depth == 0 || node.Map.HasPlayerWon(player))
                return node.Heuristic(player);

            if (player == Owner.Me)
            {
                var childNodes = getChildNodes(node.Map, Owner.Me);
                foreach (var child in childNodes)
                {
                    alpha = Math.Max(alpha, browseTree(child, depth - 1, alpha, beta, Owner.Opponent));
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
                    beta = Math.Min(beta, browseTree(child, depth - 1, alpha, beta, Owner.Me));
                    if (beta < alpha)
                        break;
                }
                return beta;
            }
        }
    }
}
