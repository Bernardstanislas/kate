using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.IO;
using Kate.Maps;

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
            client.GameEnd += new EventHandler(this.onGameEnd);
            client.GameDisconnection += new EventHandler(this.onGameDisconnection);
            #endregion

            client.declareName(new DeclareName(name));
        }

        #region Event listeners
        public virtual void onMapSet(object sender, MapSetEventArgs mapSetEventArgs) 
        {
            map = new Map(mapSetEventArgs.Width, mapSetEventArgs.Height);
            Console.WriteLine("Bot: Map has been set");
        }

        public virtual void onMapInit(object sender, MapUpdateEventArgs mapUpdateEventArgs)
        {
            applyMapModifications(mapUpdateEventArgs);
            Console.WriteLine("Bot: Init update processed");
        }

        public virtual void onMapUpdate(object sender, MapUpdateEventArgs mapUpdateEventArgs)
        {
            applyMapModifications(mapUpdateEventArgs);
            Console.WriteLine("Bot: Update processed");

            client.executeMoves(playTurn());
        }

        public virtual void onGameEnd(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Bot: Game Over");
        }

        public virtual void onGameDisconnection(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("Bot: Connection with the server is over");
        }
        #endregion

        public abstract ICollection<Move> playTurn();

        private void applyMapModifications(MapUpdateEventArgs mapUpdateEventArgs)
        {
            foreach (var tile in mapUpdateEventArgs.NewTiles)
                map.setTile(tile);
        }
    }
}
