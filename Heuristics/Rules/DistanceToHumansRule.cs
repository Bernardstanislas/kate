using System;
using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public class DistanceToHumansRule : IScoringRule
    {
        public float EvaluateScore (IMap map)
        {
            int minDistance = int.MaxValue;
            foreach (var tile in map.GetGrid())
            {
                if (tile.Owner.Equals(Owner.Me))
                {
                    int distance = getClosestHumanDistance (map, tile);
                    if (distance < minDistance)
                        minDistance = distance;
                }
            }
            var mapDimension = map.GetMapDimension(); 
            int maxPossibleDistance = mapDimension[0] + mapDimension[1];

            if (minDistance.Equals(int.MaxValue))
            {
                return 1.0F;
            }
            else
            {
                return 1.0F - 2.0F * ((float)minDistance) / ((float)maxPossibleDistance);
            }
        }

        private static int getClosestHumanDistance(IMap map, Tile tile)
        {
            int minDistance = int.MaxValue;
            foreach (var candidate in map.GetGrid())
            {
                if (candidate.Owner.Equals(Owner.Humans))
                {
                    int distance = Math.Abs (tile.X - candidate.X) + Math.Abs (tile.Y - candidate.Y);
                    if (distance < minDistance)
                        minDistance = distance;
                }
            }
            return minDistance;
        }
    }
}

