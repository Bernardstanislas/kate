using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public class PopulationDifferenceRule : IScoringRule
    {
        public float EvaluateScore(IMap map)
        {
            float myPopulation = 0;
            float enemyPopulation = 0;
            foreach (var tile in map.GetGrid())
            {
                if (tile.Owner == Owner.Me)
                    myPopulation += tile.Population;
                else if (tile.Owner == Owner.Opponent)
                    enemyPopulation += tile.Population;
            }

            if (myPopulation + enemyPopulation == 0)
                return 0;
            else
                return (myPopulation - enemyPopulation) / (myPopulation + enemyPopulation);
        }
    }
}
