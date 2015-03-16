using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Utils;
using Kate.Types;

namespace Kate.Maps
{
    public static class MapUpdaterFactory
    {
        // This method will create MapUpdaters from a list of moves
        public static List<MapUpdater> Generate(Move[] moves)
        {
            var SameDestMoves = new Dictionary<Tile, List<Move>>();

            // The Moves list split in sub list, with the same destination tile. 
            foreach(Move move in moves)
            {
                if(SameDestMoves.ContainsKey(move.Dest))
                    SameDestMoves[move.Dest].Add(move);
                else
                {
                    var list = new List<Move>();
                    list.Add(move);
                    SameDestMoves.Add(move.Dest, list);
                }
            }

            var output = new List<MapUpdater>();
            foreach(List<Move> moveList in SameDestMoves.Values)
                output.AddRange(GenerateSameDestination(moveList));

            return output;
        }
        
        // This method returns MapUpdaters for a list of Moves with the same destination Tile
        private static List<MapUpdater> GenerateSameDestination(List<Move> moves)
        {
            var output = new List<MapUpdater>();
            int popToMove = 0;
            Tile destTile = new Tile(moves[0].Dest);
            Owner originalOwner = moves[0].Origin.Owner; // We just set up this tile as a way to get the original ownership
            
            // We process each move and create MapUpdaters for the origin tiles
            foreach (Move move in moves)
            {
                Tile oriTile = new Tile(move.Origin);
                popToMove += move.PopToMove;

                // Compute MapUpdaters for Original Tiles.
                switch (oriTile.Owner)
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
            switch (destTile.Owner)
            {
            case Owner.Me:
                if (originalOwner.Equals(Owner.Me))
                {
                    MapUpdater mUM = new MapUpdater(destTile.X, destTile.Y, 0, popToMove, 0);
                    output.Add(mUM);
                }
                else
                {
                    Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Owner, destTile.Population));
                    if (fightResult.Owner.Equals(Owner.Me))
                    {
                        MapUpdater mUM = new MapUpdater(destTile.X, destTile.Y, 0, fightResult.Population - destTile.Population, 0);
                        output.Add(mUM);
                    }
                    else
                    {
                        MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, 0, -destTile.Population, fightResult.Population);
                        output.Add(mUO);
                    }
                }
                break;

            case Owner.Humans:
                if (originalOwner.Equals(Owner.Opponent))
                {
                    Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Owner, destTile.Population));
                    if (fightResult.Owner.Equals(Owner.Opponent))
                    {
                        MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, -destTile.Population, 0, fightResult.Population);
                        output.Add(mUO);
                    }
                    else
                    {
                        MapUpdater mUH = new MapUpdater(destTile.X, destTile.Y,fightResult.Population - destTile.Population, 0, 0);
                        output.Add(mUH);
                    }
                }
                else
                {
                    Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Owner, destTile.Population));
                    if (fightResult.Owner.Equals(Owner.Me))
                    {
                        MapUpdater mUM = new MapUpdater(destTile.X, destTile.Y, -destTile.Population, fightResult.Population, 0);
                        output.Add(mUM);
                    }
                    else
                    {
                        MapUpdater mUH = new MapUpdater(destTile.X, destTile.Y, fightResult.Population - destTile.Population, 0, 0);
                        output.Add(mUH);
                    }
                }
                break;

            case Owner.Opponent:
                if (originalOwner.Equals(Owner.Opponent))
                {
                    MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, 0, 0, popToMove);
                    output.Add(mUO);   
                }
                else
                {
                    Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Owner, destTile.Population));
                    if (fightResult.Owner.Equals(Owner.Opponent))
                    {
                        MapUpdater mUM = new MapUpdater(destTile.X, destTile.Y, 0, 0, fightResult.Population - destTile.Population);
                        output.Add(mUM);
                    }
                    else
                    {
                        MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, 0, fightResult.Population, -destTile.Population);
                        output.Add(mUO);
                    }
                }
                break;

            case Owner.Neutral:
                if (originalOwner.Equals(Owner.Opponent))
                {
                    MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, 0, 0, popToMove);
                    output.Add(mUO);
                }
                else
                {
                    MapUpdater mUM = new MapUpdater(destTile.X, destTile.Y, 0, popToMove, 0);
                    output.Add(mUM);
                }
                break;
            }
            return output;
        }
    }
}
