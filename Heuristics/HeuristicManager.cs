﻿using System;
using System.Collections.Generic;

using Kate.Heuristics.Rules;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristics
{
    public static class HeuristicManager 
    {
        private static HeuristicDictionary weightedRules = new HeuristicDictionary(new Dictionary<IScoringRule, int> 
        {
            {new DistanceToEnemiesRule(), 12},
            {new DistanceToHumansRule(), 3},
            {new GroupsDifferenceRule(), 2},
            {new PopulationDifferenceRule(), 8}
        });

        public static float GetScore(IMap map)
        {
            float score = 0;
            foreach (var weightedRule in weightedRules)
                score += weightedRule.Key.EvaluateScore(map) * weightedRule.Value;
            return score / weightedRules.TotalWeight;
        }
    }

    public class HeuristicDictionary : Dictionary<IScoringRule, int>
    {
        public int TotalWeight { get; private set; }

        public HeuristicDictionary(Dictionary<IScoringRule, int> weightedRules)
        {
            TotalWeight = 0;
            foreach (var weightedRule in weightedRules)
            {
                Add(weightedRule.Key, weightedRule.Value);
                TotalWeight += weightedRule.Value;
            }
        }
    }
}
