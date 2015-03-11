using System.Collections.Generic;
using System.Linq;

using Kate.Maps;
using Kate.Types;
using Kate.Heuristic;

namespace Kate.Bots.Workers
{
    public class DefaultWorker : AbstractWorker
    {
        public DefaultWorker(IMap map, Owner turn) : base(map, turn) { }

        public override IEnumerable<TreeNode> ComputeNodeChildren()
        {
            var scoreFn = HeuristicManager.Instance.GetScore;
            return generateMapPerNode().Select(map => new TreeNode(map.Item1, scoreFn, map.Item2));
        }
    }
}

