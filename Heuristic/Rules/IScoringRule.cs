using System;

using Kate.Maps;

namespace Kate.Heuristic.Rules
{
    public interface IScoringRule
    {
        decimal evaluateScore(IMap map);
    }
}

