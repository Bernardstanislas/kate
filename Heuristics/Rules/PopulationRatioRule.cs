using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public class PopulationRatioRule : IScoringRule
    {
        public float EvaluateScore(IMap map, Owner player)
        {
            int myPopulation = 0;
            int enemyPopulation = 0;
            foreach (var tile in map.getGrid())
            {
                if (tile.Owner == player)
                    myPopulation += tile.Population;
                else if (tile.Owner == player.Opposite())
                    enemyPopulation += tile.Population;
            }

            return (float) enemyPopulation / (float) myPopulation;
        }
    }
}
