using System;
using Engine;
using Models.Map;

namespace Bot
{
	public abstract class AbstractBot: IBot
	{
		protected readonly IClient client;
		protected IMap currentMap;

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
		public virtual void onMapSet(object sender, MapSetEventArgs mapSetEventArgs) {
			if (currentMap == null) {
				currentMap = new Map (mapSetEventArgs.XDimension, mapSetEventArgs.YDimension);
			} else {
				throw new ArgumentException ("Trying to set the size of the map but it already exists.");
			}
		}
        public virtual void onMapInitialization(object sender, MapUpdateEventArgs mapInitializationEventArgs)
        {
            applyMapModifications(mapInitializationEventArgs, currentMap); 
        }
        public virtual void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventsArgs)
        {
            applyMapModifications(mapUpdateEventsArgs, currentMap);
        }
		public abstract void onHomeSet(object sender, MapUpdateEventArgs homeSetEventArgs);
		public abstract void onHousesSet(object sender, MapUpdateEventArgs housesSetEventArgs);
		public abstract void onGameEnd(object sender, EventArgs eventArgs);
		public abstract void onDisconnection(object sender, EventArgs eventArgs);
		#endregion

        private void applyMapModifications(MapUpdateEventArgs mapInitializationEventArgs, IMap targetMap)
        {
            foreach (MapUpdater mapUpdater in mapInitializationEventArgs.MapUpdaters)
            {
                mapUpdater.execute(targetMap);
            } 
        }
	}
}

