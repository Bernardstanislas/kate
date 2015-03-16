using System;
using System.Collections.Generic;
using System.Linq;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Bots.Workers
{
    public abstract class AbstractWorker : IWorker
    {
        public IMap Map { get; private set; }
        public Owner Turn { get; private set; }

        protected AbstractWorker(IMap map, Owner turn)
        {
            Map = map;
            Turn = turn;
        }

        public abstract IEnumerable<TreeNode> ComputeNodeChildren();

        protected virtual IEnumerable<Tuple<IMap, List<Move>>> generateMapPerNode()
        {
            return getMapUpdatersPerNode().Select(mapUpdaters =>
            {
                IMap map = new Map((Map)Map);
                foreach (var mapUpdater in mapUpdaters.Item1)
                    map.UpdateMap(mapUpdater);

                return Tuple.Create(map, mapUpdaters.Item2);
            });
        }

        private IEnumerable<Tuple<List<MapUpdater>, List<Move>>> getMapUpdatersPerNode()
        {
            var movesListsPerNode = Map.GenerateMovesLists(Turn);
            return movesListsPerNode.Select(moveList => Tuple.Create(MapUpdaterFactory.Generate(moveList), moveList.ToList()));
        }
    }
}
