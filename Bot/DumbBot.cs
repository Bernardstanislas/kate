using System;
using System.Collections.Generic;
using Engine;
using Models.Map;
using Models.Commands;
using Models.Player;

namespace Bot
{
    public class DumbBot : AbstractBot
    {
        public DumbBot(IClient client) : base(client)
        {
            client.MapUpdate += new MapUpdateEventHandler(this.playTurn);
            this.startBot();
        }

        public override void onGameEnd(object sender, EventArgs eventArgs)
        {

        }
        
        public override void onDisconnection(object sender, EventArgs eventArgs)
        {

        }
        
        private void playTurn(object sender, MapUpdateEventArgs mapUpdateEventsArgs)
        {
            Random rnd = new Random();
            List<Tile> myTiles = new List<Tile>();

            foreach (Tile tile in currentMap.getGrid())
            {
                if (tile.Owner.Equals(Models.Player.ME))
                {
                    int proba = rnd.Next(1, 11);
                    if (proba > 5)
                    {
                        myTiles.Add(tile);
                    }
                }
            }

            //TODO : process the tiles and create the MOVE commands.
        }

        private void startBot()
        {
            client.open();
            DeclareName declareName = new DeclareName();
            client.declareName(declareName);

        }

    }
}
