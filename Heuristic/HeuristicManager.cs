using System;
using System.Collections.Generic;

using Kate.Heuristic.Rules;
using Kate.Maps;

namespace Kate.Heuristic
{
    public class HeuristicManager
    {
        private Dictionary<IScoringRule, int> weightedRules;

        public HeuristicManager(Dictionary<IScoringRule, int> weightedRules)
        {
            this.weightedRules = weightedRules;
        }

        public float getScore(IMap map)
        {
            float score = 0.0f;
            foreach (var weightedRule in weightedRules)
                score += weightedRule.Key.evaluateScore(map) + weightedRule.Value;

            score = score / getTotalWeight();
            return score;
        }

        private int getTotalWeight()
        {
            int total = 0;
            foreach (var weightedRule in weightedRules)
                total += weightedRule.Value;

            return total;
        }
    }
}
