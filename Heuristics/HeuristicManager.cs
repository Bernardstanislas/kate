using System;
using System.Collections.Generic;

using Kate.Heuristics.Rules;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristics
{
    public sealed class HeuristicManager 
    {
        public Func<IMap, float> GetScore { get; private set; }

        private HeuristicDictionary weightedRules;

        private static readonly Lazy<HeuristicManager> lazy = new Lazy<HeuristicManager>(() => new HeuristicManager());
        public static HeuristicManager Instance { get { return lazy.Value; } }
        private HeuristicManager()
        {
            weightedRules = new HeuristicDictionary(new Dictionary<IScoringRule, int> 
            {
                {new DistanceToEnemiesRule(), 1},
                {new DistanceToHumansRule(), 1},
                {new GroupsDifferenceRule(), 1},
                {new PopulationDifferenceRule(), 1},
            });

            GetScore = (IMap map) =>
            {
                float score = 0;
                foreach (var weightedRule in weightedRules)
                    score += weightedRule.Key.EvaluateScore(map) * weightedRule.Value;
                return score / weightedRules.TotalWeight;
            };
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
