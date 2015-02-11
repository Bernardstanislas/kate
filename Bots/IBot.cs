using System;
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
		#endregion

        void playTurn(object sender, MapUpdateEventArgs mapUpdateEventArgs);
	}
}

