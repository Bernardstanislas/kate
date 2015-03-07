using System;
using System.Collections.Generic;

using Kate.Maps;
using Kate.Types;

namespace Kate.Bots
{
    [Serializable()]
    public class TreeNode
    {
        public IMap Map { get; private set; }
        public Dictionary<Owner, float> Heuristics { get; private set; }
        public List<int> ChildrenHashes { get; private set; }

        public TreeNode(IMap value) : this(value, new float[2] {0,0}, new List<int>()) { }
        public TreeNode(IMap value, float[] heuristics) : this(value, heuristics, new List<int>()) { }
        public TreeNode(IMap value, float[] heuristics, List<int> children)
        {
            Map = value;
            Heuristics = new Dictionary<Owner, float> { { Owner.Me, heuristics[0] }, { Owner.Opponent, heuristics[1] } };
            ChildrenHashes = children;
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
