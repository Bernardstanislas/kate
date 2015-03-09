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
        public DefaultWorker(IMap map, Owner turn) : base(map, turn) { }

        public override List<TreeNode> ComputeNodeChildren()
        {
            var mapPerNode = generateMapPerNode();
            var treeNodes = new ConcurrentBag<TreeNode>();

            Parallel.ForEach(mapPerNode, item =>
                treeNodes.Add(new TreeNode(item.Item1, item.Item2, HeuristicManager.Instance.GetScore))
            );

            return new List<TreeNode>(treeNodes);
        }
    }
}

