using System;
using System.Collections.Generic;

using Kate.Heuristic.Rules;
using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristic
{
    public class HeuristicManager
    {
        private Dictionary<IScoringRule, int> weightedRules;

        public HeuristicManager(Dictionary<IScoringRule, int> weightedRules)
        {
            this.weightedRules = weightedRules;
        }

        public float[] getScore(IMap map)
        {
            float myScore = 0.0f;
            float enemyScore = 0.0f;
            foreach (var weightedRule in weightedRules)
            {
                myScore += weightedRule.Key.evaluateScore(map, Owner.Me) + weightedRule.Value;
                enemyScore += weightedRule.Key.evaluateScore(map, Owner.Opponent) + weightedRule.Value;
            }

            myScore = myScore / getTotalWeight();
            enemyScore = enemyScore / getTotalWeight();
            return new float[2] {myScore, enemyScore};
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
