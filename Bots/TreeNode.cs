using System;
using System.Collections.Generic;

using Kate.Commands;
using Kate.Maps;
using Kate.Types;

namespace Kate.Bots
{
    [Serializable()]
    public class TreeNode
    {
        private Func<IMap, Owner, float> heuristic;

        public IMap Map { get; private set; }
        public List<Move> MoveList { get; private set; }
        public Func<Owner, float> Heuristic { get; private set; }
        public List<int> ChildrenHashes { get; private set; }

        public TreeNode(IMap value, Func<IMap, Owner, float> heuristic) : this(value, heuristic, new List<Move>()) { }
        public TreeNode(IMap value, Func<IMap, Owner, float> heuristic, List<Move> moveList)
        {
            Map = value;
            MoveList = moveList;
            this.heuristic = heuristic;
            Heuristic = player => this.heuristic(Map, player); 
            ChildrenHashes = new List<int>();
        }

        public override int GetHashCode()
        {
            return Map.GetHashCode();
        }

        public void AddChildren(int childrenHash)
        {
            ChildrenHashes.Add(childrenHash);
        }
    }
}
