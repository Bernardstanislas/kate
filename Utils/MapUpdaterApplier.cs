using System;
using System.Collections.Generic;
using Kate.Maps;
using Kate.Types;
using Kate.Commands;


namespace Kate.Utils
{
    public static class MapUpdaterApplier
    {
        //This method will create MapUpdaters from a list of moves
        public static List<MapUpdater> Apply(List<Move> moves)
        {
            Dictionary<Tile, List<Move>> dict = new Dictionary<Tile, List<Move>>();
            var output = new List<MapUpdater>();

            //The Moves list split in sub list, with the same destination tile. 
            foreach(Move move in moves)
            {
                if(dict.ContainsKey(move.Dest))
                {
                    dict[move.Dest].Add(move);
                }
                else
                {
                    List<Move> list = new List<Move>();
                    list.Add(move);
                    dict.Add(move.Dest, list);
                }
            }

            foreach(List<Move> moveList in dict.Values)
            {
                List<MapUpdater> buff = new List<MapUpdater>(MapUpdaterApplier.ApplySameDestination(moveList));
                
                foreach(MapUpdater mU in buff)
                {
                    output.Add(mU);
                }
            }
            return output;
        }
        
        //this method returns map updaters for a list of 
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

            //We then need to return one map updater for the destination tile


            return output;
 
        }
    }
}
