using System;
using System.Collections.Generic;
using System.Linq;

using Kate.Types;

namespace Kate.Maps
{
    public class Map : AbstractMap
    {
        private Tile[,] grid;
        private int[, ,] hashArray;

        public Map (int xSize, int ySize)
        {
            grid = new Tile[xSize, ySize];

            for (int i = 0; i < xSize; i++)
                for (int j = 0; j < ySize; j++)
                    grid[i, j] = new Tile(i, j);
        }

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
            return new int[2] { grid.GetLength(0), grid.GetLength(1) };
        }

        public override void setTile(Tile newTile) 
        {
            grid[newTile.X, newTile.Y] = newTile;
        }

        public override Tile getTile(int xCoordinate, int yCoordinate) 
        {
            return grid[xCoordinate, yCoordinate];
        }

        public override int GetHashCode()
        {
            var hashArray = new int[grid.GetLength(0),grid.GetLength(1)];

            for (int x = 0; x < grid.GetLength(0); x++)
                for (int y = 0; y < grid.GetLength(1); y++)
                    if (grid[x, y].Owner != Owner.Neutral)
                        hashArray[x, y] = grid[x, y].Population;

            return 0;
        }

        private int[, ,] generateHashArray()
        {
            Random random = new Random();

            var hashArray = new int[grid.GetLength(0), grid.GetLength(1), 3];

            for (int player = 0; player < (int)Owner.Neutral; player++)
                for (int x = 0; x < grid.GetLength(0); x++)
                    for (int y = 0; y < grid.GetLength(1); y++)
                        hashArray[x, y, player] = random.Next();

            return hashArray;
        }

        private override void computeHash()
        {
            int hash = 0;

            for (int x = 0; x < grid.GetLength(0); x++)
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    var tile = getTile(x, y);
                    if (tile.Owner != Owner.Neutral)
                        hash ^= hashArray[x, y, (int)tile.Owner];
                }
        }
    }
}

