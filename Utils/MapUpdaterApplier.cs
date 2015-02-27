using System;
using System.Collections.Generic;
using Kate.Maps;
using Kate.Types;
using Kate.Commands;


namespace Kate.Utils
{
    public static class MapUpdaterApplier
    {

        private static List<MapUpdater> ApplySameDestination(List<Move> moves)
        {
            List<MapUpdater> output = new List<MapUpdater>();
            int popToMove = 0;
            Tile destTile = new Tile(moves[0].Dest);
            //We just set up this tile as a way to get the original ownership
            Owner originOwnership = moves[0].Origin.Owner;

            // We process each move and create MapUpdaters for the origin tiles
            foreach (Move move in moves )
            {
                Tile oriTile = new Tile(move.Origin);
                //oriTile.Population -= move.PopToMove;
                popToMove += move.PopToMove;

                //Compute MapUpdaters for Original Tiles.
                switch (originOwnership)
                {
                    case Owner.Me:
                        MapUpdater mUM = new MapUpdater(oriTile.X, oriTile.Y, 0, -move.PopToMove, 0);
                        output.Add(mUM);
                        break;

                    case Owner.Humans:
                        MapUpdater mUH = new MapUpdater(oriTile.X, oriTile.Y, -move.PopToMove, 0, 0);
                        output.Add(mUH);
                        break;
                    case Owner.Opponent:
                        MapUpdater mUO = new MapUpdater(oriTile.X, oriTile.Y, 0, 0, -move.PopToMove);
                        output.Add(mUO);
                        break;
                        
                }

            }

            return output;
 
        }
    }
}
