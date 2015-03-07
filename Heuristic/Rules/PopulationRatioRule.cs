using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristic.Rules
{
    public class PopulationRatioRule : IScoringRule
    {
        public float evaluateScore(IMap map, Owner player)
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

            return (float) myPopulation / (float) enemyPopulation;
        }
    }
}
