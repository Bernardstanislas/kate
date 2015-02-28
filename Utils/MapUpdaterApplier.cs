using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;

namespace Kate.Utils
{
    public static class MapUpdaterApplier
    {
        // This method will create MapUpdaters from a list of moves
        public static List<MapUpdater> Apply(List<Move> moves)
        {
            var dict = new Dictionary<Tile, List<Move>>();

            // The Moves list split in sub list, with the same destination tile. 
            foreach(Move move in moves)
            {
                if(dict.ContainsKey(move.Dest))
                    dict[move.Dest].Add(move);
                else
                {
                    List<Move> list = new List<Move>();
                    list.Add(move);
                    dict.Add(move.Dest, list);
                }
            }

            var output = new List<MapUpdater>();
            foreach(List<Move> moveList in dict.Values)
                output.AddRange(ApplySameDestination(moveList));

            return output;
        }
        
        // This method returns a MapUpdater list for a list of Moves
        private static List<MapUpdater> ApplySameDestination(List<Move> moves)
        {
            var output = new List<MapUpdater>();
            int popToMove = 0;
            Tile destTile = new Tile(moves[0].Dest);
            // We just set up this tile as a way to get the original ownership
            Owner originOwnership = moves[0].Origin.Owner;

            // We process each move and create MapUpdaters for the origin tiles
            foreach (Move move in moves)
            {
                Tile oriTile = new Tile(move.Origin);
                // oriTile.Population -= move.PopToMove;
                popToMove += move.PopToMove;

                // Compute MapUpdaters for Original Tiles.
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
            // We then need to return one map updater for the destination tile
            // ==> You don't need to. That's why we're working with MapUpdaters instead of Tiles.
            // What you need to do instead is to use your FightUtil there.

            return output;
        }
    }
}
