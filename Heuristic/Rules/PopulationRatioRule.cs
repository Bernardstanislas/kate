using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristic.Rules
{
    public class PopulationRatioRule : IScoringRule
    {
        public decimal evaluateScore(IMap map)
        {
            int myPopulation = 0;
            int enemyPopulation = 0;
            foreach (var tile in map.getGrid())
            {
                switch (tile.Owner)
                {
                case Owner.Me:
                    myPopulation += tile.Population;
                    break;
                case Owner.Opponent:
                    enemyPopulation += tile.Population;
                    break;
                }
            }

            return Decimal.Divide(myPopulation, enemyPopulation);
        }
    }
}

