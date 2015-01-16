using System;

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
		void close();
	}
}

