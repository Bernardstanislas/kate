using System;
using System.Collections.Generic;
using Engine;
using Models.Map;
using Models.Commands;
using Models;

namespace Bot
{
    public class DumbBot : AbstractBot
    {
        public DumbBot(IClient client, IMap map) : base(client, map)
        {
            client.MapUpdate += new MapUpdateEventHandler(this.playTurn);
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
            int[] gridDim = map.getMapDimension();

            //We get our own tiles and they got a 50% chance of being processed, ie put into the myTiles list
            foreach (Tile tile in map.getGrid())
            {
                if (tile.Owner.Equals(Models.Player.Me))
                    myTiles.Add(tile);
            }

            // We create a Move list which will be passed to the client
            List<Move> turnMoves = new List<Move>();

            //I feel like I'm missing something there, I got no clue on how to tell the MapUpdater how to move the ppl
            foreach (Tile tile in myTiles)
            {
                //we randomly determine the number of ppl to move for each tile
                int popToMove = rnd.Next(1, tile.Population);

                //Then we randomly choose a direction to move
                int xPos = tile.XCoordinate;
                int yPos = tile.YCoordinate;

                int proba = rnd.Next(1, 11);
                if (proba > 5)
                { 
                    if (xPos < gridDim[0] - 1)
                        xPos += 1;
                    else
                        xPos -= 1;
                }
                else 
                {
                    if (yPos < gridDim[1] - 1)
                        yPos += 1;
                    else
                        yPos -= 1;
                }

                Tile destTile = map.getTile(xPos, yPos);
    
                Move move = new Move(tile, destTile, popToMove);
                turnMoves.Add(move);
            }

            client.executeMoves(turnMoves);
        }
    }
}
