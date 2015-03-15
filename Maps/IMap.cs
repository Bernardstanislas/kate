using System;
using System.Collections.Generic;

using Kate.Types;
using Kate.Commands;

namespace Kate.Maps
{
    public interface IMap
    {
        IEnumerable<Tile> getGrid();
        int[] getMapDimension();
        Tile getTile (int xCoordinate, int yCoordinate);
        IEnumerable<Tile> getPlayerTiles(Owner owner);
        void setTile (Tile newTile);
        void updateMap(IMapUpdater mapUpdater);
        bool HasGameEnded();
        List<List<Move>> GenerateMovesLists(Owner owner);
    }
}
