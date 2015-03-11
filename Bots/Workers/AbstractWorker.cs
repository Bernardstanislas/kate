using System;
using System.Collections.Generic;

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
            foreach (var mapUpdaters in getMapUpdatersPerNode()) 
            {
				IMap map = new Map((Map)Map);
                foreach (var mapUpdater in mapUpdaters.Item1)
                    map.updateMap(mapUpdater);

                yield return Tuple.Create(map, mapUpdaters.Item2);
            }
        }

        private IEnumerable<Tuple<List<MapUpdater>, List<Move>>> getMapUpdatersPerNode()
        {
            var movesListsPerNode = MoveGenerator.GenerateMoves(Map, Turn);

            foreach (var moveList in movesListsPerNode)
                yield return Tuple.Create(MapUpdaterFactory.Generate(moveList), moveList);
        }
    }
}
