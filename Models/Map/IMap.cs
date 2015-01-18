using System;

namespace Models.Map
{
	public interface IMap
	{
		Tile getTile (int xCoordinate, int yCoordinate);
		void setTile (Tile newTile);
		void updateMap(IMapUpdater mapUpdater);
	}
}

