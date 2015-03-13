using System;
using System.Collections.Generic;

using Kate.Types;

namespace Kate.Maps
{
    [Serializable()]
    public abstract class AbstractMap: IMap
    {
        #region implemented
        protected int hash = 0;

        public override int GetHashCode()
        {
            return hash;
        }

        public virtual void updateMap(IMapUpdater mapUpdater)
        {
            mapUpdater.execute(this);
        }

        public void setTile(Tile newTile)
        {
            updateHash(newTile);
            updateTile(newTile);
        }
        #endregion

        #region abstract
        protected abstract void updateTile(Tile newTile);
        protected abstract void updateHash(Tile newTile);
        public abstract IEnumerable<Tile> getGrid();
        public abstract Tile getTile (int xCoordinate, int yCoordinate);
        public abstract IEnumerable<Tile> getPlayerTiles(Owner owner);
        public abstract int[] getMapDimension();
        public abstract bool HasGameEnded();
        #endregion
    }
}
