using System;

namespace Models.Map
{
	public interface IMapUpdater
	{
		void execute(IMap target);
	}
}

