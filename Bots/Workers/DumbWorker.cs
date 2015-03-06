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

        public override List<TreeNode<IMap>> ComputeNodeChildren()
        {
            return new List<TreeNode<IMap>>()
            { 
                new TreeNode<IMap>(Map, 0),
                new TreeNode<IMap>(Map, 0),
                new TreeNode<IMap>(Map, 0)
            };
        }
    }
}
