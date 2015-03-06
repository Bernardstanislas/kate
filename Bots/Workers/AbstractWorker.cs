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

        public abstract List<TreeNode> ComputeNodeChildren();

        protected virtual List<IMap> GenerateMapPerNode()
        {
            var mapPerNode = new List<IMap>();
            foreach (var mapUpdaters in GetMapUpdatersPerNode()) 
            {
				var map = new Map((Map)Map);
                foreach (var mapUpdater in mapUpdaters)
                    map.updateMap(mapUpdater);

                mapPerNode.Add(map);
            }
            return mapPerNode;
        }

        private List<List<MapUpdater>> GetMapUpdatersPerNode()
        {
            var movesListsPerNode = MoveGenerator.GenerateMoves(Map, Turn);

            var mapUpdatersPerNode = new List<List<MapUpdater>>();
            foreach (var moveList in movesListsPerNode)
                mapUpdatersPerNode.Add(MapUpdaterFactory.Generate(moveList));
            
            return mapUpdatersPerNode;
        }
    }
}
