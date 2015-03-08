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
        public Dictionary<Owner, float> Heuristics { get; private set; }
        public List<int> ChildrenHashes { get; private set; }

        public TreeNode(IMap value) : this(value, new List<Move>(), new float[2] {0,0}) { }
        public TreeNode(IMap value, List<Move> moveList, float[] heuristics)
        {
            Map = value;
            MoveList = moveList;
            Heuristics = new Dictionary<Owner, float> 
            { 
                { Owner.Me, heuristics[0] }, 
                { Owner.Opponent, heuristics[1] } 
            };
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
