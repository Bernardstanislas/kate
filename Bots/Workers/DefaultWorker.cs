using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

using Kate.Maps;
using Kate.Types;
using Kate.Heuristic;
using Kate.Heuristic.Rules;

namespace Kate.Bots.Workers
{
    public class DefaultWorker : AbstractWorker
    {
        private static readonly HeuristicManager heuristicManager = new HeuristicManager(new Dictionary<IScoringRule, int>{
            {new PopulationRatioRule(), 1},
            {new TotalPopulationRule(), 1}
        });

        public DefaultWorker (IMap map, Owner turn) : base (map, turn) {}

        public override List<TreeNode> ComputeNodeChildren()
        {
            List<IMap> mapPerNode = generateMapPerNode ();
            var treeNodes = new ConcurrentBag<TreeNode> ();

            Parallel.ForEach (mapPerNode, item => {
                treeNodes.Add (new TreeNode (item, heuristicManager.getScore (item)));
            });

            return new List<TreeNode> (treeNodes);
        }
    }
}

