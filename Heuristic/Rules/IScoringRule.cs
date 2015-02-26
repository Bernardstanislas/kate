using System;

using Kate.Maps;

namespace Kate.Heuristic.Rules
{
    public interface IScoringRule
    {
        float evaluateScore(IMap map);
    }
}

