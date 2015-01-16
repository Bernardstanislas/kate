using System;
using Engine;

namespace Bot
{
	public interface IBot
	{
		#region Event listeners
		void onMapSet(object sender, MapSetEventArgs mapSetEventArgs);
		void onMapInitialization(object sender, MapInitializationEventArgs mapInitializationEventArgs);
		void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventsArgs);
		void onHomeSet(object sender, HomeSetEventArgs homeSetEventArgs);
		void onHousesSet(object sender, HousesSetEventArgs housesSetEventArgs);
		void onGameEnd(object sender, EventArgs eventArgs);
		void onDisconnection(object sender, EventArgs eventArgs);
		#endregion
	}
}

