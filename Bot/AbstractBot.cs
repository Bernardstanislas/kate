using System;
using Engine;

namespace Bot
{
	public abstract class AbstractBot: IBot
	{
		protected readonly IClient client;

		protected AbstractBot(IClient client)
		{
			this.client = client;
			#region Events subcriptions
			client.MapSet += new MapSetEventHandler(this.onMapSet);
			client.MapInitialization += new MapInitializationEventHandler(this.onMapInitialization);
			client.MapUpdate += new MapUpdateEventHandler(this.onMapInitialization);
			client.HomeSet += new HomeSetEventHandler(this.onHomeSet);
			client.HousesSet += new HousesSetEventHandler(this.onHousesSet);
			client.GameEnd += new EventHandler(this.onGameEnd);
			client.Disconnection += new EventHandler(this.onDisconnection);
			#endregion
		}

		#region Event listeners
		protected virtual void onMapSet(object sender, MapSetEventArgs mapSetEventArgs);
		protected virtual void onMapInitialization(object sender, MapInitializationEventArgs mapInitializationEventArgs);
		protected virtual void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventsArgs);
		protected virtual void onHomeSet(object sender, HomeSetEventArgs homeSetEventArgs);
		protected virtual void onHousesSet(object sender, HousesSetEventArgs housesSetEventArgs);
		protected virtual void onGameEnd(object sender, EventArgs eventArgs);
		protected virtual void onDisconnection(object sender, EventArgs eventArgs);
		#endregion
	}
}

