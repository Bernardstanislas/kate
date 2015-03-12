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
        public IEnumerable<int> ChildrenHashes { get; private set; }

        public TreeNode(IMap value, Func<IMap, Owner, float> heuristic) : this(value, heuristic, new List<Move>()) { }
        public TreeNode(IMap value, Func<IMap, Owner, float> heuristic, List<Move> moveList) : this(value, heuristic, moveList, new List<int>()) { }
        public TreeNode(IMap value, Func<IMap, Owner, float> heuristic, List<Move> moveList, IEnumerable<int> childrenHashes)
        {
            Map = value;
            MoveList = moveList;
            this.heuristic = heuristic;
            Heuristic = player => this.heuristic(Map, player);
            ChildrenHashes = childrenHashes;
        }

        public override int GetHashCode()
        {
            return Map.GetHashCode();
        }
    }
}
