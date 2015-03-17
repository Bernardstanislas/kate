using System;
using System.Linq;

using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristics.Rules
{
    public class DistanceToEnemiesRule : IScoringRule
    {
        public float EvaluateScore(IMap map)
        {
            var mapDimension = map.GetMapDimension();

            float maxPossibleDistance = Math.Max(mapDimension[0], mapDimension[1]);
            float evaluationCount = 0;
            float score = 0;

            var myTiles = map.GetPlayerTiles(Owner.Me);
            if (myTiles.Count() == 0)
                return -1;

            foreach (var myTile in map.GetPlayerTiles(Owner.Me))
                foreach (var enemyTile in map.GetPlayerTiles(Owner.Opponent))
                {
                    evaluationCount++;
                    float distance = getLTwoDistance(myTile, enemyTile);
                    if (FightUtil.IsWon(myTile.Population, myTile.Owner, enemyTile.Population, enemyTile.Owner))
                        score += 1 - distance / maxPossibleDistance;
                    else
                        score += distance / maxPossibleDistance - 1;
                }

            if (evaluationCount == 0)
                return 1;
            else
                return Math.Max(-1, Math.Min(1, score / evaluationCount));
        }

        private static float getLTwoDistance(Tile tile1, Tile tile2)
        {
            return (float) Math.Sqrt((tile1.X - tile2.X) * (tile1.X - tile2.X) + (tile1.Y - tile2.Y) * (tile1.Y - tile2.Y));
        }
    }
}

