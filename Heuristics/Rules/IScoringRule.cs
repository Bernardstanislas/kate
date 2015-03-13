using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public interface IScoringRule
    {
        float EvaluateScore(IMap map);
    }
}
