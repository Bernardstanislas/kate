using System;
using System.Collections.Generic;

using Kate.Heuristics.Rules;
using Kate.Maps;
using Kate.Types;
using Kate.Utils;

namespace Kate.Heuristics
{
    public sealed class HeuristicManager : AbstractHeuristicManager
    {
        private static readonly Lazy<HeuristicManager> lazy = new Lazy<HeuristicManager>(() => new HeuristicManager());
        public static HeuristicManager Instance { get { return lazy.Value; } }
        private HeuristicManager()
        {
            weightedRules = new HeuristicDictionary(new Dictionary<IScoringRule, int> 
            {
                {new PopulationDifferenceRule(), 1},
                {new GroupsDifferenceRule(), 1}
            });

            createGetScore();
        }
    }
}
