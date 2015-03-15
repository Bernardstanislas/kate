﻿using System;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristics.Rules
{
    public class VictoriesRule : IScoringRule
    {
        public float EvaluateScore (IMap map)
        {
            int maxPossibleDistance = map.getMapDimension()[0] + map.getMapDimension()[1];
            int evaluationCount = 0;
            float score = 0f;
            foreach (var tile in map.getGrid())
            {
                if (tile.Owner.Equals(Owner.Me))
                {
                    foreach (var otherTile in map.getGrid())
                    {
                        if (otherTile.Owner.Equals(Owner.Opponent))
                        {
                            evaluationCount++;
                            int distance = getManhattanDistance (tile, otherTile);
                            if (FightUtil.IsWon(tile.Population, tile.Owner, otherTile.Population, otherTile.Owner))
                            {
                                score += 1 - ((float)distance) / ((float)maxPossibleDistance);
                            }
                            else
                            {
                                score += ((float)distance) / ((float)maxPossibleDistance) - 1;
                            } 
                        }
                    }
                }
            }

            return score / evaluationCount;
        }

        private static int getManhattanDistance(Tile tile1, Tile tile2)
        {
            return Math.Abs (tile1.X - tile2.X) + Math.Abs (tile1.Y - tile2.Y);
        }
    }
}

