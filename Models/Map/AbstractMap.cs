﻿using System;
using System.Collections.Generic;

namespace Models.Map
{
	[Serializable()]
	public abstract class AbstractMap: IMap
	{
        public abstract IEnumerable<Tile> getGrid();		
        public abstract void setTile(Tile newTile);
		public abstract Tile getTile (int xCoordinate, int yCoordinate);

		public virtual void updateMap (IMapUpdater mapUpdater)
		{
			mapUpdater.execute (this);
		}
	}
}

