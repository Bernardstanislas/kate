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
        public IMap Map { get; private set; }
        public List<Move> MoveList { get; private set; }
        public Func<Owner, float> Heuristic { get; private set; }
        public List<int> ChildrenHashes { get; private set; }

        public TreeNode(IMap value) : this(value, new List<Move>(), null) { }
        public TreeNode(IMap value, List<Move> moveList, Func<IMap, Owner, float> heuristic)
        {
            Map = value;
            MoveList = moveList;
            Heuristic = player => heuristic(Map, player); 
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
