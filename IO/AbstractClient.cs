using System;
using Kate.Commands;
using Kate.Maps;
using System.Collections.Generic;

namespace Kate.IO
{
	public abstract class AbstractClient: IClient
	{
		#region Events management
		#region Fired events
		// A client notifies its observers when a message is received from the engine server
		public event MapSetEventHandler MapSet;
        public event MapInitEventHandler MapInit;
		public event MapUpdateEventHandler MapUpdate;
		public event EventHandler GameEnd;
		#endregion

		#region Events launchers
		// Fires a MapInitialization event when called
		protected virtual void OnMapSet(MapSetEventArgs mapSetEventArgs)
		{
			if (MapSet != null)
				MapSet(this, mapSetEventArgs);
		}

		// Fires a MapInit event when called
		protected virtual void OnMapInit(MapUpdateEventArgs mapInitEventArgs)
		{
			if (MapInit != null)
				MapInit(this, mapInitEventArgs);
		}

		// Fires a MapUpdate event when called
		protected virtual void OnMapUpdate(MapUpdateEventArgs mapUpdateEventsArgs)
		{
			if (MapUpdate != null)
				MapUpdate(this, mapUpdateEventsArgs);
		}

		// Fires a GameEnd event when called
		protected virtual void OnGameEnd(EventArgs eventArgs)
		{
			if (GameEnd != null)
				GameEnd(this, eventArgs);
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
	public delegate void MapInitEventHandler(object sender, MapUpdateEventArgs e);
	public delegate void MapUpdateEventHandler(object sender, MapUpdateEventArgs e);
	#endregion

	#region Custom event args
	// Custom EventArgs children made to pass data between the client and the bot
	public class MapSetEventArgs: EventArgs
	{
		public int Width { get; private set; }
		public int Height { get; private set; }

        public MapSetEventArgs(int width, int height) : base()
        {
            Width = width;
            Height = height;
        }
	}

	public class MapUpdateEventArgs: EventArgs
	{
        public ICollection<Tile> NewTiles { get; private set; }

		public MapUpdateEventArgs(ICollection<Tile> newTiles)
		{
            NewTiles = newTiles;
		}
	}
	#endregion
}
