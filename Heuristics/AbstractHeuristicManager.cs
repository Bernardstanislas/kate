using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kate.Heuristics.Rules;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristics
{
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

    public abstract class AbstractHeuristicManager
    {
        public Func<IMap, float> GetScore { get; protected set; }

        protected HeuristicDictionary weightedRules;

        protected void createGetScore()
        {
            GetScore = (IMap map) =>
            {
                float score = 0;
                foreach (var weightedRule in weightedRules)
                    score += weightedRule.Key.EvaluateScore(map) * weightedRule.Value;
                return score / weightedRules.TotalWeight;
            };
            //GetScore = GetScore.Memoize();
        }
    }
}
