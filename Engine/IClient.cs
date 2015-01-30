using System;
using Models.Commands;
using System.Collections.Generic;

namespace Engine
{
	public interface IClient
	{
		event MapSetEventHandler MapSet;
		event MapInitializationEventHandler MapInitialization;
		event MapUpdateEventHandler MapUpdate;
		event HomeSetEventHandler HomeSet;
		event HousesSetEventHandler HousesSet;
		event EventHandler GameEnd;
		event EventHandler Disconnection;

		void open();
		void declareName(DeclareName declareName);
		void executeMoves (ICollection<Move> moves);
		void close();
	}
}

