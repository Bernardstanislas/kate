using System.Collections.Generic;

namespace Kate.Bots
{
    public class TreeNode<T>
    {
        public T Value { get; private set; }
        public float Heuristic { get; private set; }
        public List<TreeNode<T>> Children { get; set; }

        public TreeNode(T value) : this(value, 0f, new List<TreeNode<T>>()) { }
		public TreeNode(T value, float heurisitic) : this(value, heurisitic, new List<TreeNode<T>>()) { }
		public TreeNode(T value, float heuristic, List<TreeNode<T>> children)
        {
            Value = value;
            Heuristic = heuristic;
            Children = children;
        }
    }
}
