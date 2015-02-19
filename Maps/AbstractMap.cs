using System;
using System.Collections.Generic;

using Kate.Types;

namespace Kate.Maps
{
    [Serializable()]
    public abstract class AbstractMap: IMap
    {
        private int hash = 0;

        public abstract IEnumerable<Tile> getGrid();		
        public abstract void setTile(Tile newTile);
        public abstract Tile getTile (int xCoordinate, int yCoordinate);
		public abstract IEnumerable<Tile> getPlayerTiles(Owner owner);
        public abstract int[] getMapDimension();

        public abstract void ComputeHash();
        public override int GetHashCode()
        {
            if (hash == 0) ComputeHash();
                return hash;
        }
        public abstract void UpdateHash(IMapUpdater mapUpdater);

        public virtual void updateMap(IMapUpdater mapUpdater)
        {
            if (hash == 0) ComputeHash();
                UpdateHash(mapUpdater);

            mapUpdater.execute(this);
        }
    }
}

