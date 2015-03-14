using System;
using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public class DistanceToHumanRule : IScoringRule
    {
        public float EvaluateScore (IMap map)
        {
            int minDistance = int.MaxValue;
            foreach (var tile in map.getGrid())
            {
                if (tile.Owner.Equals(Owner.Me))
                {
                    int distance = getClosestHumanDistance (map, tile);
                    if (distance < minDistance)
                        minDistance = distance;
                }
            }
            int maxPossibleDistance = map.getMapDimension [0] + map.getMapDimension [1];
            return ((float)minDistance) / ((float)maxPossibleDistance);
        }

        private static int getClosestHumanDistance(IMap map, Tile tile)
        {
            int minDistance = int.MaxValue;
            foreach (var candidate in map.getGrid())
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

