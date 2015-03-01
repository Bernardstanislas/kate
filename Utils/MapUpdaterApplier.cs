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
            Owner originalOwner = moves[0].Origin.Owner;
            //We just set up this tile as a way to get the original ownership

            // We process each move and create MapUpdaters for the origin tiles
            foreach (Move move in moves )
            {
                Tile oriTile = new Tile(move.Origin);
                popToMove += move.PopToMove;

                //Compute MapUpdaters for Original Tiles.
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

            //We then need to return one map updater for the destination tile
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
                        Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Population, destTile.Owner));
                        if (fightResult.Owner.Equals(Owner.Me))
                        {
                            MapUpdater mUM = new MapUpdater(destTile.X, destTile.Y, 0, destTile.Population-fightResult.Population, 0);
                            output.Add(mUM);
                        }
                        else
                        {
                            MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, 0, - destTile.Population, fightResult.Population);
                            output.Add(mUO);
                        }
                    }
                    break;

                case Owner.Humans:
                    if (originalOwner.Equals(Owner.Opponent))
                    {
                        Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Population, destTile.Owner));
                        if (fightResult.Owner.Equals(Owner.Opponent))
                        {
                            MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, -destTile.Population, 0, fightResult.Population);
                            output.Add(mUO);
                        }
                        else
                        {
                            MapUpdater mUH = new MapUpdater(destTile.X, destTile.Y, destTile.Population-fightResult.Population, 0, 0);
                            output.Add(mUH);
                        }
                    }
                    else
                    {
                        Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Population, destTile.Owner));
                        if (fightResult.Owner.Equals(Owner.Me))
                        {
                            MapUpdater mUO = new MapUpdater(destTile.X, destTile.Y, -destTile.Population, fightResult.Population, 0);
                            output.Add(mUO);
                        }
                        else
                        {
                            MapUpdater mUH = new MapUpdater(destTile.X, destTile.Y, destTile.Population - fightResult.Population, 0, 0);
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
                        Tile fightResult = new Tile(FightUtil.FightResult(originalOwner, popToMove, destTile.Population, destTile.Owner));
                        if (fightResult.Owner.Equals(Owner.Opponent))
                        {
                            MapUpdater mUM = new MapUpdater(destTile.X, destTile.Y, 0, 0, destTile.Population - fightResult.Population);
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
