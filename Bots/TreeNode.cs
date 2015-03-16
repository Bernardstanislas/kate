using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Heuristics;
using Kate.Maps;
using Kate.Types;

namespace Kate.Bots
{
    [Serializable()]
    public class TreeNode
    {
        public IMap Map { get; private set; }
        public List<Move> MoveList { get; private set; }
        public float Heuristic { get; private set; }
        public IEnumerable<int> MyChildrenHashes { get; private set; }
        public IEnumerable<int> EnemyChildrenHashes { get; private set; }

        public TreeNode(IMap value) : this(value, new List<Move>()) { }
        public TreeNode(IMap value, List<Move> moveList)
        {
            Map = value;
            Heuristic = HeuristicManager.Instance.GetScore(Map);
            MoveList = moveList;
            MyChildrenHashes = new List<int>();
            EnemyChildrenHashes = new List<int>();
        }
        
        public IEnumerable<int> GetChildrenHashes(Owner player)
        {
            if (player == Owner.Me)
                return MyChildrenHashes;
            else
                return EnemyChildrenHashes;
        }

        public void AddChildren(IEnumerable<int> childrenHashes, Owner player)
        {
            if(player == Owner.Me)
                MyChildrenHashes = childrenHashes;
            else
                EnemyChildrenHashes = childrenHashes;
        }

        public override int GetHashCode()
        {
            return Map.GetHashCode();
        }
    }
}
