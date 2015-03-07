﻿using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristic.Rules
{
    public class TotalPopulationRule : IScoringRule
    {
        public float evaluateScore(IMap map, Owner player)
        {
            int myPopulation = 0;
            int totalPopulation = 0;
            foreach (var tile in map.getGrid())
            {
                if (tile.Owner == player)
                    myPopulation += tile.Population;

                totalPopulation += tile.Population;
            }
            return (float) myPopulation / (float) totalPopulation;
        }
    }
}
