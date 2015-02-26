using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristic.Rules
{
    public class TotalPopulationRule : IScoringRule
    {
        public decimal evaluateScore (IMap map)
        {
            int myPopulation = 0;
            int totalPopulation = 0;
            foreach (var tile in map.getGrid ())
            {
                if (tile.Owner.Equals(Owner.Me))
                    myPopulation += tile.Population;

                totalPopulation += tile.Population;
            }
            return Decimal.Divide (myPopulation, totalPopulation);
        }
    }
}

