﻿using System;
using System.Collections.Generic;

namespace Models.Map
{
	public class Map : AbstractMap
	{
        private Tile[,] grid;

		public Map (int xSize, int ySize)
		{
			grid = new Tile[xSize,ySize];
		}

        public override IEnumerable<Tile> getGrid()
        {
            foreach (Tile tile in grid)
            {
                yield return tile;
            }
        }

        public override int[] getMapDimension()
        {
            return new int[2] { grid.GetLength(0), grid.GetLength(1) };
        }

		public override void setTile(Tile newTile) {
			grid [newTile.XCoordinate, newTile.YCoordinate] = newTile;
		}

		public override Tile getTile (int xCoordinate, int yCoordinate) {
			return grid [xCoordinate, yCoordinate];
		}
	}
}
