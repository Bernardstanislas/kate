using System;
using Kate.Maps;
using Kate.Types;

namespace Kate.Heuristics.Rules
{
    public class GroupsDifferenceRule : IScoringRule
    {
        public float EvaluateScore (IMap map)
        {
            int myGroups = 0;
            int enemyGroups = 0;
            foreach (var tile in map.GetGrid())
            {
                if (tile.Owner == Owner.Me)
                    myGroups += 1;
                else if (tile.Owner == Owner.Opponent)
                    enemyGroups += 1;
            }

            if ((myGroups + enemyGroups).Equals(0))
            {
                return 0;
            } else {
                return ((float) (myGroups - enemyGroups)) / ((float) (myGroups + enemyGroups));
            }

        }
    }
}

