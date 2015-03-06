using System;
using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Bots.Workers
{
    class DumbWorker : AbstractWorker
    {
        public DumbWorker(IMap map, Owner turn) : base(map, turn) { }

        public override List<TreeNode> ComputeNodeChildren()
        {
            return new List<TreeNode>()
            { 
                new TreeNode(Map, 0),
                new TreeNode(Map, 0),
                new TreeNode(Map, 0)
            };
        }
    }
}
