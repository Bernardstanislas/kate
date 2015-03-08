using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;

namespace Kate.IO
{
    public abstract class AbstractClient: IClient
    {
        public event MapSetEventHandler MapSet;
        public event MapInitEventHandler MapInit;
        public event MapUpdateEventHandler MapUpdate;
        public event EventHandler GameEnd;
        public event EventHandler GameDisconnection;

        public abstract void DeclareName(DeclareName declareName);
        public abstract void ExecuteMoves(ICollection<Move> moves);

        #region Events launchers
        protected void onMapSet(MapSetEventArgs mapSetEventArgs)
        {
            if (MapSet != null)
                MapSet(this, mapSetEventArgs);
        }

        protected void onMapInit(MapUpdateEventArgs mapInitEventArgs)
        {
            if (MapInit != null)
                MapInit(this, mapInitEventArgs);
        }

        protected void onMapUpdate(MapUpdateEventArgs mapUpdateEventsArgs)
        {
            if (MapUpdate != null)
                MapUpdate(this, mapUpdateEventsArgs);
        }

        protected void onGameEnd(EventArgs eventArgs)
        {
            if (GameEnd != null)
                GameEnd(this, eventArgs);
        }

        protected void onGameDisconnection(EventArgs eventArgs)
        {
            if (GameDisconnection != null)
                GameDisconnection(this, eventArgs);
        }
        #endregion       
    }
}
