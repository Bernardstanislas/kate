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

            //We get our own tiles and they got a 50% chance of being processed, ie put into the myTiles list
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

            // We create a Move list which will be passed to the client
            List<Move> turnMoves = new List<Move>();

            //I feel like I'm missing something there, I got no clue on how to tell the MapUpdater how to move the ppl
            foreach (Tile tile in myTiles)
            {
                int popToMove = rnd.Next(1, tile.Population);

                MapUpdater mapUpdater = new MapUpdater(tile);
                
                Move move = new Move(mapUpdater);
                turnMoves.Add(move);
            }

            client.executeMoves(turnMoves);
        }

        private void startBot()
        {
            client.open();
            DeclareName declareName = new DeclareName();
            client.declareName(declareName);

        }

    }
}
