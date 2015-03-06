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

        public abstract List<TreeNode<IMap>> computeNodeChildren();

		protected virtual List<IMap> generateMapPerNode()
		{
			var mapPerNode = new List<IMap> ();
			foreach(var mapUpdaters in getMapUpdatersPerNode())
			{
				var map = DeepCloneUtil.DeepClone ((Map) Map); // Cast is here to force the type of Map property
				foreach(var mapUpdater in mapUpdaters)
				{
					map.updateMap (mapUpdater);
				}
				mapPerNode.Add (map);
			}
			return mapPerNode;
		}

		private List<List<MapUpdater>> getMapUpdatersPerNode()
		{
			var movesListsPerNode = MoveGenerator.GenerateMoves (Map, Owner.Me);
			var mapUpdatersPerNode = new List<List<MapUpdater>> ();
			foreach (var moveList in movesListsPerNode)
			{
				mapUpdatersPerNode.Add (MapUpdaterFactory.Generate (moveList));
			}
			return mapUpdatersPerNode;
		}
    }
}
