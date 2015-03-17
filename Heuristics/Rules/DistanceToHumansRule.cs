using System;

using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristics.Rules
{
    public class DistanceToHumansRule : IScoringRule
    {
        public float EvaluateScore(IMap map)
        {
            float minDistance = float.MaxValue;
            foreach (var tile in map.GetGrid())
            {
                if (tile.Owner == Owner.Me)
                {
                    float distance = getClosestHumanDistance(map, tile);
                    if (distance < minDistance)
                        minDistance = distance;
                }
            }
            var mapDimension = map.GetMapDimension(); 
            float maxPossibleDistance = Math.Max(mapDimension[0], mapDimension[1]);

            if (minDistance == float.MaxValue)
                return 1;
            else
                return 1 - 2 * minDistance / maxPossibleDistance;
        }

        private static float getClosestHumanDistance(IMap map, Tile tile)
        {
            float minDistance = float.MaxValue;
            foreach (var candidate in map.GetPlayerTiles(Owner.Humans))
            {
                float distance = getLTwoDistance(tile, candidate);
                if (distance < minDistance && FightUtil.IsWon(tile.Population, tile.Owner, candidate.Population, candidate.Owner))
                    minDistance = distance;
            }

            return minDistance;
        }

        private static float getLTwoDistance(Tile tile1, Tile tile2)
        {
            return (float)Math.Sqrt((tile1.X - tile2.X) * (tile1.X - tile2.X) + (tile1.Y - tile2.Y) * (tile1.Y - tile2.Y));
        }
    }
}

