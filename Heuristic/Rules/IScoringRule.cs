﻿using System;

using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristic.Rules
{
    public interface IScoringRule
    {
        float evaluateScore(IMap map, Owner player);
    }
}
