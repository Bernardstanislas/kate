﻿using System;
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
            Tuple<Tile, Tile> output = new Tuple<Tile, Tile>(newSource, newDest);

            //attacking humans
            if ( destTile.Owner.Equals(Models.Player.Humans) )
            {
                if ( sourceTile.Population >= destTile.Population  )
                {
                    // In this case, the fight is won. So the source Tile is now an empty neutral tile
                    output.Item1.Owner = Models.Player.Neutral;
                    output.Item1.Population = 0;

                    // The destination tile becomes the player property and the population of both tiles are added
                    output.Item2.Owner = Models.Player.Me;
                    output.Item2.Population = sourceTile.Population + destTile.Population;
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

                        output.Item1.Owner = Models.Player.Neutral;
                        output.Item1.Population = 0;

                        // The destination tile becomes the player property and the population is the surviving one
                        output.Item2.Owner = Models.Player.Me;
                        output.Item2.Population = survivingPop;
                    }

                    //lost case
                    else
                    {
                        //source tile is now empty
                        output.Item1.Owner = Models.Player.Neutral;
                        output.Item1.Population = 0;

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

                        output.Item2.Owner = Models.Player.Humans;
                        output.Item2.Population = survivingPop;
                    }
                }
            }

            //attacking opponents
            else if ( destTile.Owner.Equals(Models.Player.Opponent) )
            {
                if (sourceTile.Population >= destTile.Population*1.5 )
                {
                    // In this case, the fight is won. So the source Tile is now an empty neutral tile
                    output.Item1.Owner = Models.Player.Neutral;
                    output.Item1.Population = 0;

                    // The destination tile becomes the player property and the population is hte source tile population
                    output.Item2.Owner = Models.Player.Me;
                    output.Item2.Population = sourceTile.Population;
                }

                else if (sourceTile.Population <= destTile.Population * 1.5)
                {
                    // In this case, the fight is lost. So the source Tile is now an empty neutral tile
                    output.Item1.Owner = Models.Player.Neutral;
                    output.Item1.Population = 0;

                    // The destination tile stays as the original destination tile
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

                        output.Item1.Owner = Models.Player.Neutral;
                        output.Item1.Population = 0;

                        // The destination tile becomes the player property and the population is the surviving one
                        output.Item2.Owner = Models.Player.Me;
                        output.Item2.Population = survivingPop;
                    }

                    //lost case
                    else
                    {
                        //source tile is now empty
                        output.Item1.Owner = Models.Player.Neutral;
                        output.Item1.Population = 0;

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

                        output.Item2.Owner = Models.Player.Opponent;
                        output.Item2.Population = survivingPop;
                    }

                }
            }

            else
            {
                //nothing happens it's just a move!
                output.Item1.Owner = Models.Player.Neutral;
                output.Item1.Population = 0;

                output.Item2.Owner = Models.Player.Me;
                output.Item2.Population = sourceTile.Population;
            }

            return output;
        }
    }
}
