using System;
using System.Collections.Generic;
using System.Linq;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;

namespace Kate.Bots
{
    public static class NodeComputer
    {
        public static IEnumerable<TreeNode> GetChildren(IMap map, Owner turn)
        {
            return map.GenerateMovesLists(turn).Select(moveList => 
                MapUpdaterFactory.Generate(moveList)
            ).Select(mapUpdaters =>
            {
                IMap newMap = new Map((Map)map);
                foreach (var mapUpdater in mapUpdaters)
                    newMap.UpdateMap(mapUpdater);

                return new TreeNode(newMap);
            });
        }

        public static IEnumerable<Tuple<TreeNode, List<Move>>> GetChildrenWithMoveList(IMap map, Owner turn)
        {
            return map.GenerateMovesLists(turn).Select(moveList =>
                Tuple.Create(MapUpdaterFactory.Generate(moveList), moveList)
            ).Select(mapUpdaters =>
            {
                IMap newMap = new Map((Map)map);
                foreach (var mapUpdater in mapUpdaters.Item1)
                    newMap.UpdateMap(mapUpdater);

                return Tuple.Create(new TreeNode(newMap), mapUpdaters.Item2.ToList());
            });
        }
    }
}
