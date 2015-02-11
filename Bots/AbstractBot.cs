using System;
using Kate.IO;
using Kate.Maps;
using Kate.Commands;

namespace Kate.Bots
{
	public abstract class AbstractBot: IBot
	{
		protected readonly IClient client;
		protected IMap map;

		protected AbstractBot(IClient client, string name)
		{
			this.client = client;

			#region Events subcriptions
			client.MapSet += new MapSetEventHandler(this.onMapSet);
			client.MapInit += new MapInitEventHandler(this.onMapInit);
			client.MapUpdate += new MapUpdateEventHandler(this.onMapUpdate);
            client.MapUpdate += new MapUpdateEventHandler(this.playTurn);
			client.GameEnd += new EventHandler(this.onGameEnd);
			#endregion

            client.declareName(new DeclareName(name));
		}

		#region Event listeners
		public virtual void onMapSet(object sender, MapSetEventArgs mapSetEventArgs) 
        {
			if (map == null)
				map = new Kate.Maps.Map(mapSetEventArgs.Width, mapSetEventArgs.Height);
			else
				throw new ArgumentException("Trying to set the size of the map but it already exists.");
		}

        public virtual void onMapInit(object sender, MapUpdateEventArgs mapUpdateEventArgs)
        {
            applyMapModifications(mapUpdateEventArgs);
        }

        public virtual void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventArgs)
        {
            applyMapModifications(mapUpdateEventArgs); 
        }

        public abstract void playTurn(object sender, MapUpdateEventArgs mapUpdateEventArgs);

		public virtual void onGameEnd(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Game Over");
        }
		#endregion

        private void applyMapModifications(MapUpdateEventArgs mapUpdateEventArgs)
        {
            foreach (var tile in mapUpdateEventArgs.NewTiles)
                map.setTile(tile);
        }
	}
}

