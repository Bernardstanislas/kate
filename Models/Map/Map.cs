using System;

namespace Models.Map
{
	public class Map : AbstractMap
	{
		private Tile[,] grid;

		public Map (int xSize, int ySize)
		{
			grid = new Tile[xSize,ySize];
		}

		public override void setTile(Tile newTile) {
			grid [newTile.XCoordinate, newTile.YCoordinate] = newTile;
		}

		public override Tile getTile (int xCoordinate, int yCoordinate) {
			return grid [xCoordinate, yCoordinate];
		}
	}
}

