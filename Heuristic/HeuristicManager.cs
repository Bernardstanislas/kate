using System;
using System.Collections.Generic;

using Kate.Heuristic.Rules;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristic
{
    public class HeuristicDictionary : Dictionary<IScoringRule, int>
    {
        public int TotalWeight { get; private set; }

        public HeuristicDictionary(Dictionary<IScoringRule, int> weightedRules)
        {
            TotalWeight = 0;
            foreach(var weightedRule in weightedRules)
            {
                Add(weightedRule.Key, weightedRule.Value);
                TotalWeight += weightedRule.Value;
            }
        }
    }

    public class HeuristicManager
    {
        private HeuristicDictionary weightedRules;

        public Func<IMap, Owner, float> GetScore { get; private set; }

        public HeuristicManager(Dictionary<IScoringRule, int> rules)
        {
            weightedRules = new HeuristicDictionary(rules);

            GetScore = (IMap map, Owner player) =>
            {
                float score = 0;
                foreach (var weightedRule in weightedRules)
                    score += weightedRule.Key.evaluateScore(map, player) + weightedRule.Value;
                return score / weightedRules.TotalWeight;
            };
            GetScore = GetScore.Memoize();
        }
    }
}
