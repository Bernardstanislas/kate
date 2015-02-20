using System;
using System.Collections.Generic;

namespace Kate.Maps
{
    public interface IMap
    {
        IEnumerable<Tile> getGrid();
        int[] getMapDimension();
        Tile getTile (int xCoordinate, int yCoordinate);
		IEnumerable<Tile> getMyTiles();
        void setTile (Tile newTile);
        void updateMap(IMapUpdater mapUpdater);
    }
}

