using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public class PopulationDifferenceRule : IScoringRule
    {
        public float EvaluateScore(IMap map)
        {
            int myPopulation = 0;
            int enemyPopulation = 0;
            foreach (var tile in map.GetGrid())
            {
                if (tile.Owner == Owner.Me)
                    myPopulation += tile.Population;
                else if (tile.Owner == Owner.Opponent)
                    enemyPopulation += tile.Population;
            }

            return (float)((myPopulation - enemyPopulation) / (myPopulation + enemyPopulation));
        }
    }
}
