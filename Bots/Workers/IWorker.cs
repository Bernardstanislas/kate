using System;
using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;

namespace Kate.Bots.Workers
{
    public interface IWorker
    {
        IMap Map { get; }
        Owner Turn { get; }

        List<TreeNode<IMap>> computeNodeChildren();
    }
}
