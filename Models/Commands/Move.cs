using System;
using Models.Map;
using System.Collections.Generic;

namespace Models.Commands
{
	public class Move
	{
		public List<IMapUpdater> Result{ get; set;}

		public Move (Tile origin, Tile dest, int popToMove)
		{


			//this.Result = result;
		}
	}
}

