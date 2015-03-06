using System;
using System.Collections.Generic;

using Kate.Maps;

namespace Kate.Bots
{
    [Serializable()]
    public class TreeNode
    {
        public IMap Map { get; private set; }
        public float Heuristic { get; private set; }
        public List<int> ChildrenHashes { get; private set; }

        public TreeNode(IMap value) : this(value, 0, new List<int>()) { }
        public TreeNode(IMap value, float heurisitic) : this(value, heurisitic, new List<int>()) { }
        public TreeNode(IMap value, float heuristic, List<int> children)
        {
            Map = value;
            Heuristic = heuristic;
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
