using System;
using Models.Map;
using System.Collections.Generic;

namespace Models.Commands
{
	public class Move
	{
		public IMapUpdater Result{ get;}

		public Move (IMapUpdater result)
		{
			this.Result = result;
		}
	}
}

