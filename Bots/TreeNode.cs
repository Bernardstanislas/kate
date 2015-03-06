using System;
using System.Collections.Generic;

namespace Kate.Bots
{
    [Serializable()]
    public class TreeNode<T>
    {
        public T Value { get; private set; }
        public int Heuristic { get; private set; }
        public List<int> ChildrenHashes { get; private set; }

        public TreeNode(T value) : this(value, 0, new List<int>()) { }
        public TreeNode(T value, int heurisitic) : this(value, heurisitic, new List<int>()) { }
        public TreeNode(T value, int heuristic, List<int> children)
        {
            Value = value;
            Heuristic = heuristic;
            ChildrenHashes = children;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public void AddChildren(int childrenHash)
        {
            ChildrenHashes.Add(childrenHash);
        }
    }
}
