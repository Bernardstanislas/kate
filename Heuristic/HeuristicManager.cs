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

    public sealed class HeuristicManager
    {
        public Func<IMap, Owner, float> GetScore { get; private set; }
        
        private HeuristicDictionary weightedRules;

        private static readonly Lazy<HeuristicManager> lazy = new Lazy<HeuristicManager>(() => new HeuristicManager());
        public static HeuristicManager Instance { get { return lazy.Value; } }
        private HeuristicManager()
        {
            weightedRules = new HeuristicDictionary(new Dictionary<IScoringRule, int> 
            {
                {new PopulationRatioRule(), 1},
                {new TotalPopulationRule(), 1}
            });

            GetScore = (IMap map, Owner player) =>
            {
                float score = 0;
                foreach (var weightedRule in weightedRules)
                    score += weightedRule.Key.EvaluateScore(map, player) + weightedRule.Value;
                return score / weightedRules.TotalWeight;
            };
            GetScore = GetScore.Memoize();
        }
    }
}
