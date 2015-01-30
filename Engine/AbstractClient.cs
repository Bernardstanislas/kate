using System;
using Models.Commands;
using Models.Map;
using System.Collections.Generic;

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
		protected virtual void OnMapInitialization(MapUpdateEventArgs mapInitializationEventArgs)
		{
			if (MapInitialization != null)
			{
				MapInitialization (this, mapInitializationEventArgs);
			}
		}

		// Fires a MapUpdate event when called
		protected virtual void OnMapUpdate(MapUpdateEventArgs mapUpdateEventsArgs)
		{
			if (MapUpdate != null)
			{
				MapUpdate (this, mapUpdateEventsArgs);
			}
		}

		// Fires a HomeSet event when called
		protected virtual void OnHomeSet(MapUpdateEventArgs homeSetEventArgs)
		{
			if (HomeSet != null)
			{
				HomeSet (this, homeSetEventArgs);
			}
		}

		// Fires a HousesSet event when called
		protected virtual void OnHousesSet(MapUpdateEventArgs housesSetEventArgs)
		{
			if (HousesSet != null)
			{
				HousesSet (this, housesSetEventArgs);
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

		// Opens the client connection to the engine server
		public abstract void open();

		// Closes the client connection to the engine server
		public abstract void close();

		// Declares the AI's name to the engine server
		public abstract void declareName(DeclareName declareName);

		// Executes the current turn's command (moves or attack)
		public abstract void executeMoves(ICollection<Move> moves);
	}

	#region Event handlers delegates
	public delegate void MapSetEventHandler(object sender, MapSetEventArgs e);
	public delegate void MapInitializationEventHandler(object sender, MapUpdateEventArgs e);
	public delegate void MapUpdateEventHandler(object sender, MapUpdateEventArgs e);
	public delegate void HomeSetEventHandler(object sender, MapUpdateEventArgs e);
	public delegate void HousesSetEventHandler(object sender, MapUpdateEventArgs e);
	#endregion

	#region Custom event args
	// Custom EventArgs children made to pass data between the client and the bot

	public class MapSetEventArgs: EventArgs
	{
		public int XDimension{ get; set;}
		public int YDimension{ get; set;}

	}

	public class MapUpdateEventArgs: EventArgs
	{
		private ICollection<IMapUpdater> mapUpdaters;

		public ICollection<IMapUpdater> MapUpdaters
		{
			get
			{
				return mapUpdaters;
			}
			set
			{
				mapUpdaters = value;
			}
		}

		public MapUpdateEventArgs(ICollection<IMapUpdater> mapUpdaters)
		{
			this.MapUpdaters = mapUpdaters;
		}
	}
	#endregion
}
