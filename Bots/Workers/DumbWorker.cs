using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Bots.Workers
{
    class DumbWorker : AbstractWorker
    {
        public DumbWorker(IMap map, Owner turn, int nodeHash) : base(map, turn, nodeHash) { }

        private ICollection<IMap> ComputeNode()
        {
            return new List<IMap>() { Map, Map, Map };
        }
    }
}
