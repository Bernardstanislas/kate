using System;
using System.Collections.Generic;

using Kate.Types;

namespace Kate.Maps
{
    [Serializable()]
    public abstract class AbstractMap: IMap
    {
        public abstract IEnumerable<Tile> getGrid();		
        public abstract void setTile(Tile newTile);
        public abstract Tile getTile (int xCoordinate, int yCoordinate);
		public abstract IEnumerable<Tile> getPlayerTiles(Owner owner);
        public abstract int[] getMapDimension();

        public virtual void updateMap (IMapUpdater mapUpdater)
        {
            mapUpdater.execute(this);
        }
    }
}

