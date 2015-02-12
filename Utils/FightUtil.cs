using System;
using System.Collections.Generic;
using Models.Map;
using Models.Player;


namespace Utils
{
    public static class FightUtil
    {
        public static Tuple <Tile, Tile> FightUtil(Tile sourceTile, Tile destTile)
        {
            Tile newSource = sourceTile;
            Tile newDest = destTile;
            Random rnd = new Random();

            //attacking humans
            if ( destTile.Owner.Equals(Models.Player.Humans) )
            {
                if ( sourceTile.Population >= destTile.Population  )
                {
                    // In this case, the fight is won. So the source Tile is now an empty neutral tile
                    newSource.Owner = Models.Player.Neutral;
                    newSource.Population = 0;

                    // The destination tile becomes the player property and the population of both tiles are added
                    newDest.Owner = Models.Player.Me;
                    newDest.Population = sourceTile.Population + destTile.Population;

                    Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                    return res;
                }

                else
                {
                    //Here we enter a random battle
                    int victoryProba = (sourceTile.Population / (sourceTile.Population + destTile.Population));
                    int fightProba = rnd.Next(1, 101);

                    //first case is a victorious one
                    if (fightProba >= victoryProba*100)
                    {
                        //we check the surviving population
                        int totalPop = sourceTile.Population + destTile.Population;
                        int survivingPop = 0;
                        for (int i = 0; i < totalPop; i++ )
                        {
                            int surviveProba = rnd.Next(1, 101);
                            if (surviveProba >= victoryProba*100)
                            {
                                survivingPop++;
                            }
                        }

                        newSource.Owner = Models.Player.Neutral;
                        newSource.Population = 0;

                        // The destination tile becomes the player property and the population is the surviving one
                        newDest.Owner = Models.Player.Me;
                        newDest.Population = survivingPop;

                        Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                        return res;
                    }

                    //lost case
                    else
                    {
                        //source tile is now empty
                        newSource.Owner = Models.Player.Neutral;
                        newSource.Population = 0;

                        //We check the remaining human population
                        int survivingPop = 0;
                        for (int i = 0; i < destTile.Population; i++)
                        {
                            int surviveProba = rnd.Next(1, 101);
                            if (surviveProba >= victoryProba * 100)
                            {
                                survivingPop++;
                            }
                        }

                        newDest.Owner = Models.Player.Humans;
                        newDest.Population = survivingPop;

                        Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                        return res;
                    }
                }
            }

            //attacking opponents
            else if ( destTile.Owner.Equals(Models.Player.Opponent) )
            {
                if (sourceTile.Population >= destTile.Population*1.5 )
                {
                    // In this case, the fight is won. So the source Tile is now an empty neutral tile
                    newSource.Owner = Models.Player.Neutral;
                    newSource.Population = 0;

                    // The destination tile becomes the player property and the population is hte source tile population
                    newDest.Owner = Models.Player.Me;
                    newDest.Population = sourceTile.Population;

                    Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                    return res;
                }

                else if (sourceTile.Population <= destTile.Population * 1.5)
                {
                    // In this case, the fight is lost. So the source Tile is now an empty neutral tile
                    newSource.Owner = Models.Player.Neutral;
                    newSource.Population = 0;

                    // The destination tile stays as the original destination tile

                    Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                    return res;
                }

                else
                {
                    //Here we enter a random battle
                    int victoryProba = (sourceTile.Population / (sourceTile.Population + destTile.Population));
                    int fightProba = rnd.Next(1, 101);

                    //first case is a victorious one
                    if (fightProba >= victoryProba * 100)
                    {
                        //we check the surviving population
                        int survivingPop = 0;
                        for (int i = 0; i < sourceTile.Population; i++)
                        {
                            int surviveProba = rnd.Next(1, 101);
                            if (surviveProba >= victoryProba * 100)
                            {
                                survivingPop++;
                            }
                        }

                        newSource.Owner = Models.Player.Neutral;
                        newSource.Population = 0;

                        // The destination tile becomes the player property and the population is the surviving one
                        newDest.Owner = Models.Player.Me;
                        newDest.Population = survivingPop;

                        Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                        return res;
                    }

                    //lost case
                    else
                    {
                        //source tile is now empty
                        newSource.Owner = Models.Player.Neutral;
                        newSource.Population = 0;

                        //We check the remaining human population
                        int survivingPop = 0;
                        for (int i = 0; i < destTile.Population; i++)
                        {
                            int surviveProba = rnd.Next(1, 101);
                            if (surviveProba >= victoryProba * 100)
                            {
                                survivingPop++;
                            }
                        }

                        newDest.Owner = Models.Player.Opponent;
                        newDest.Population = survivingPop;

                        Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                        return res;
                    }

                }
            }

            else
            {
                //nothing happens it's just a move!
                newSource.Owner = Models.Player.Neutral;
                newSource.Population = 0;

                newDest.Owner = Models.Player.Me;
                newDest.Population = sourceTile.Population;

                Tuple<Tile, Tile> res = new Tuple<Tile, Tile>(newSource, newDest);
                return res;
            }
        }
    }
}
