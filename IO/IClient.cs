using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;

namespace Kate.IO
{
    public interface IClient
    {
        event MapSetEventHandler MapSet;
        event MapInitEventHandler MapInit;
        event MapUpdateEventHandler MapUpdate;
        event EventHandler GameEnd;
        event EventHandler GameDisconnection;

        void DeclareName(DeclareName declareName);
        void ExecuteMoves(ICollection<Move> moves);
    }

    #region Event handlers
    public delegate void MapSetEventHandler(object sender, MapSetEventArgs e);
    public delegate void MapInitEventHandler(object sender, MapUpdateEventArgs e);
    public delegate void MapUpdateEventHandler(object sender, MapUpdateEventArgs e);
    public delegate void GameEnd(object sender, EventArgs e);
    public delegate void GameDisconnectionEventHandler(object sender, EventArgs e);
    #endregion

    #region Event args
    // Custom EventArgs children made to pass data between the client and the bot
    public class MapSetEventArgs : EventArgs
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        public MapSetEventArgs(int width, int height) : base()
        {
            Width = width;
            Height = height;
        }
    }

    public class MapUpdateEventArgs : EventArgs
    {
        public ICollection<Tile> NewTiles { get; private set; }

        public MapUpdateEventArgs(ICollection<Tile> newTiles)
        {
            NewTiles = newTiles;
        }
    }
    #endregion
}
