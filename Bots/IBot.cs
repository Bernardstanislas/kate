using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.IO;

namespace Kate.Bots
{
    public interface IBot
    {
        #region Event listeners
        void onMapSet(object sender, MapSetEventArgs mapSetEventArgs);
        void onMapInit(object sender, MapUpdateEventArgs mapInitializationEventArgs);
        void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventArgs);
        void onGameEnd(object sender, EventArgs eventArgs);
        void onGameDisconnection(object sender, EventArgs eventArgs);
        #endregion

        ICollection<Move> playTurn();
    }
}

