using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.IO;
using Kate.Maps;

namespace Kate.Bots
{
    public abstract class Bot
    {
        protected readonly IClient client;
        protected IMap map;

        public Bot(IClient client, string name)
        {
            this.client = client;

            #region Event registration
            client.MapSet += new MapSetEventHandler(this.onMapSet);
            client.MapInit += new MapInitEventHandler(this.onMapInit);
            client.MapUpdate += new MapUpdateEventHandler(this.onMapUpdate);
            client.GameEnd += new EventHandler(this.onGameEnd);
            client.GameDisconnection += new EventHandler(this.onGameDisconnection);
            #endregion

            client.declareName(new DeclareName(name));
        }

        protected abstract ICollection<Move> playTurn();

        #region Event listeners
        private void onMapSet(object sender, MapSetEventArgs mapSetEventArgs) 
        {
            map = new Map(mapSetEventArgs.Width, mapSetEventArgs.Height);
            Console.WriteLine("Bot: Map has been set");
        }

        private void onMapInit(object sender, MapUpdateEventArgs mapUpdateEventArgs)
        {
            applyMapModifications(mapUpdateEventArgs);
            Console.WriteLine("Bot: Init update processed");
        }

        private void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventArgs)
        {
            applyMapModifications(mapUpdateEventArgs);
            Console.WriteLine("Bot: Update processed");

            client.executeMoves(playTurn());
        }

        private void onGameEnd(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Bot: Game Over");
        }

        private void onGameDisconnection(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Bot: Connection with the server is over");
        }
        #endregion

        #region Map updating
        private void applyMapModifications(MapUpdateEventArgs mapUpdateEventArgs)
        {
            foreach (var tile in mapUpdateEventArgs.NewTiles)
                map.setTile(tile);
        }
        #endregion
    }
}
