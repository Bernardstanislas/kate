using System;
using System.Collections.Generic;

using Kate.Types;

namespace Kate.Maps
{
    public class Map : AbstractMap
    {
        private Tile[,] grid;
        private int[, , ,] hashArray;
        private bool isHashArrayGenerated = false;

        public Map (int xSize, int ySize)
        {
            generateHashArray(xSize, ySize);

            grid = new Tile[xSize, ySize];
            for (int i = 0; i < grid.GetLength (0); i++)
                for (int j = 0; j < grid.GetLength (1); j++) {
                    grid [i, j] = new Tile (i, j);
                    hash ^= hashArray [i, j, (int)Owner.Neutral, 0];
                }
        }

        #region Grid
        public override IEnumerable<Tile> getGrid()
        {
            foreach (Tile tile in grid)
                yield return tile;
        }

        public override IEnumerable<Tile> getPlayerTiles(Owner owner)
        {
            foreach (Tile tile in grid)
                if (tile.Owner.Equals(owner))
                    yield return tile;
        }

        public override int[] getMapDimension()
        {
            return new int[2] {grid.GetLength(0), grid.GetLength(1)};
        }

        protected override void updateTile(Tile newTile) 
        {
            grid[newTile.X, newTile.Y] = newTile;
        }

        public override Tile getTile(int xCoordinate, int yCoordinate) 
        {
            return grid[xCoordinate, yCoordinate];
        }
        #endregion

        #region Hash
        private void generateHashArray(int xSize, int ySize)
        {
            Random random = new Random();
            hashArray = new int[xSize, ySize, Enum.GetNames(typeof(Owner)).Length, 256];

            for (int index0 = 0; index0 < hashArray.GetLength(0); index0++)
                for (int index1 = 0; index0 < hashArray.GetLength(1); index1++)
                    for (int index2 = 0; index0 < hashArray.GetLength(2); index2++)
                        for (int index3 = 0; index0 < hashArray.GetLength(3); index3++)
                            hashArray[index0, index1, index2, index3] = random.Next();
        }

        protected override void updateHash(Tile newTile)
        {
            var oldTile = getTile(newTile.X, newTile.Y);
            hash = hash 
                ^ hashArray[oldTile.X, oldTile.Y, (int)oldTile.Owner, oldTile.Population] 
                ^ hashArray[newTile.X, newTile.Y, (int)newTile.Owner, newTile.Population];
        }
        #endregion
    }
}
