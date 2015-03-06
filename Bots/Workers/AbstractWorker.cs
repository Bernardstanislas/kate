using System;
using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;

namespace Kate.Bots.Workers
{
    public abstract class AbstractWorker : IWorker
    {
        public IMap Map { get; private set; }
        public Owner Turn { get; private set; }

        public AbstractWorker(IMap map, Owner turn)
        {
            Map = map;
            Turn = turn;
        }

        public abstract List<TreeNode> ComputeNodeChildren();
    }
}
