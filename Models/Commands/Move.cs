using System;
using Models.Map;
using System.Collections.Generic;

namespace Models.Commands
{
	public class Move
	{
		private ICollection<IMapUpdater> mapUpdaters;

		public ICollection<IMapUpdater> MapUpdaters{ get; set;}

		public Move (ICollection<IMapUpdater> mapUpdaters)
		{
			this.mapUpdaters = mapUpdaters;
		}
	}
}

