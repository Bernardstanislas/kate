using System;
using System.Collections.Generic;

using Kate.Types;

namespace Kate.Maps
{
    [Serializable()]
    public abstract partial class AbstractMap : IMap
    {
        #region implemented
        protected int hash = 0;

        public override int GetHashCode()
        {
            return hash;
        }

        public virtual void UpdateMap(IMapUpdater mapUpdater)
        {
            mapUpdater.execute(this);
        }

        public void SetTile(Tile newTile)
        {
            UpdateHash(newTile);
            UpdateTile(newTile);
        }
        #endregion

        #region abstract
        protected abstract void UpdateTile(Tile newTile);
        protected abstract void UpdateHash(Tile newTile);
        public abstract IEnumerable<Tile> GetGrid();
        public abstract Tile GetTile(int xCoordinate, int yCoordinate);
        public abstract IEnumerable<Tile> GetPlayerTiles(Owner owner);
        public abstract IEnumerable<Tile> GetSurroundingTiles(Tile tile);
        public abstract int[] GetMapDimension();
        public abstract bool HasGameEnded();
        #endregion
    }
}
