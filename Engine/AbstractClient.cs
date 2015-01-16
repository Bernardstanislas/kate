using System;

namespace Engine
{
	public abstract class AbstractClient: IClient
	{
		#region Events management
		#region Fired events
		// A client notifies its observers when a message is received from the engine server
		public event MapSetEventHandler MapSet;
		public event MapInitializationEventHandler MapInitialization;
		public event MapUpdateEventHandler MapUpdate;
		public event HomeSetEventHandler HomeSet;
		public event HousesSetEventHandler HousesSet;
		public event EventHandler GameEnd;
		public event EventHandler Disconnection;
		#endregion

		#region Events launchers
		// Fires a MapInitialization event when called
		protected virtual void OnMapSet(MapSetEventArgs mapSetEventArgs)
		{
			if (MapSet != null)
			{
				MapSet (this, mapSetEventArgs);
			}
		}

		// Fires a MapInitialization event when called
		protected virtual void OnMapInitialization(MapInitializationEventArgs mapInitializationEventArgs)
		{
			if (MapInitialization != null)
			{
				MapInitialization (this, mapInitializationEventArgs);
			}
		}

		// Fires a MapUpdate event when called
		protected virtual void OnMapUpdate(MapUpdateEventArgs mapUpdateEventsArgs)
		{
			if (MapUpdateEventHandler != null)
			{
				MapUpdateEventHandler (this, mapUpdateEventsArgs);
			}
		}

		// Fires a HomeSet event when called
		protected virtual void OnHomeSet(HomeSetEventArgs homeSetEventArgs)
		{
			if (HomeSet != null)
			{
				HomeSet (this, homeSetEventArgs);
			}
		}

		// Fires a HousesSet event when called
		protected virtual void OnHousesSet(HousesSetEventArgs housesSetEventArgs)
		{
			if (HousesSetEventHandler != null)
			{
				HousesSetEventHandler (this, housesSetEventArgs);
			}
		}

		// Fires a GameEnd event when called
		protected virtual void OnGameEnd(EventArgs eventArgs)
		{
			if (GameEnd != null)
			{
				GameEnd (this, eventArgs);
			}
		}

		// Fires a Disconnection event when called
		protected virtual void OnDisconnection(EventArgs eventArgs)
		{
			if (Disconnection != null)
			{
				Disconnection (this, eventArgs);
			}
		}
		#endregion
		#endregion

		// Open the client connection to the engine server
		public virtual void open();

		// Close the client connection to the engine server
		public virtual void close();
	}

	#region Event handlers delegates
	public delegate void MapSetEventHandler(object sender, MapSetEventArgs e);
	public delegate void MapInitializationEventHandler(object sender, MapInitializationEventArgs e);
	public delegate void MapUpdateEventHandler(object sender, MapUpdateEventArgs e);
	public delegate void HomeSetEventHandler(object sender, HomeSetEventArgs e);
	public delegate void HousesSetEventHandler(object sender, HousesSetEventArgs e);
	#endregion

	#region Custom event args
	// Custom EventArgs children made to pass data between the client and the bot

	public class MapSetEventArgs: EventArgs
	{
		//TODO add the relevant data to the event args
	}

	public class MapInitializationEventArgs: EventArgs
	{
		//TODO add the relevant data to the event args
	}

	public class MapUpdateEventArgs: EventArgs
	{
		//TODO add the relevant data to the event args
	}

	public class HomeSetEventArgs: EventArgs
	{
		//TODO add the relevant data to the event args
	}

	public class HousesSetEventArgs: EventArgs
	{
		//TODO add the relevant data to the event args
	}
	#endregion
}
