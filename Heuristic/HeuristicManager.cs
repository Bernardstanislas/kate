using System;
using Kate.Maps;
using System.Collections.Generic;
using Kate.Heuristic.Rules;

namespace Kate.Heuristic
{
	public class HeuristicManager
	{
		private Dictionary<IScoringRule, int> weightedRules;

		public HeuristicManager (Dictionary<IScoringRule, int> weightedRules)
		{
			this.weightedRules = weightedRules;
		}

		public decimal getScore(IMap map)
		{
			decimal score = 0.0m;
			foreach (var weightedRule in weightedRules)
			{
				score += weightedRule.Key.evaluateScore (map) + weightedRule.Value;
			}
			score = score / getTotalWeight ();
			return score;
		}

		private int getTotalWeight()
		{
			int total = 0;
			foreach (var weightedRule in weightedRules)
			{
				total += weightedRule.Value;
			}
			return total;
		}
	}
}

