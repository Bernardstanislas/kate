using System;
using System.Collections.Generic;
using Kate.Types;

namespace Kate.Maps
{
    public class Map : AbstractMap
    {
        private Tile[,] grid;

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

		public override IEnumerable<Tile> getMyTiles()
        {
			foreach (Tile tile in grid)
				if (tile.Owner.Equals(Owner.Me))
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
    }
}

