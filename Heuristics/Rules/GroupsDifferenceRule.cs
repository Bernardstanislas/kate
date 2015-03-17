using System;
using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public class GroupsDifferenceRule : IScoringRule
    {
        public float EvaluateScore (IMap map)
        {
            float myGroups = 0;
            float enemyGroups = 0;
            foreach (var tile in map.GetGrid())
            {
                if (tile.Owner == Owner.Me)
                    myGroups += 1;
                else if (tile.Owner == Owner.Opponent)
                    enemyGroups += 1;
            }

            if (myGroups + enemyGroups == 0)
                return 0;
            else
                return (enemyGroups - myGroups) / (myGroups + enemyGroups);

        }
    }
}

