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
			client.MapUpdate += new MapUpdateEventHandler(this.onMapUpdate);
			client.HomeSet += new HomeSetEventHandler(this.onHomeSet);
			client.HousesSet += new HousesSetEventHandler(this.onHousesSet);
			client.GameEnd += new EventHandler(this.onGameEnd);
			client.Disconnection += new EventHandler(this.onDisconnection);
			#endregion
		}

		#region Event listeners
		public abstract void onMapSet(object sender, MapSetEventArgs mapSetEventArgs);
		public abstract void onMapInitialization(object sender, MapUpdateEventArgs mapInitializationEventArgs);
		public abstract void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventsArgs);
		public abstract void onHomeSet(object sender, MapUpdateEventArgs homeSetEventArgs);
		public abstract void onHousesSet(object sender, MapUpdateEventArgs housesSetEventArgs);
		public abstract void onGameEnd(object sender, EventArgs eventArgs);
		public abstract void onDisconnection(object sender, EventArgs eventArgs);
		#endregion
	}
}

