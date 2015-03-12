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
        public Func<Owner, float> Heuristic { get; private set; }
        public IEnumerable<int> MyChildrenHashes { get; private set; }
        public IEnumerable<int> EnemyChildrenHashes { get; private set; }

        public TreeNode(IMap value) : this(value, new List<Move>()) { }
        public TreeNode(IMap value, List<Move> moveList) : this(value, moveList, new List<int>(), new List<int>()) { }
        public TreeNode(IMap value, List<Move> moveList, IEnumerable<int> myChildrenHashes, IEnumerable<int> enemyChildrenHashes)
        {
            Map = value;
            Heuristic = player => HeuristicManager.Instance.GetScore(Map, player);
            MoveList = moveList;
            MyChildrenHashes = myChildrenHashes;
            EnemyChildrenHashes = enemyChildrenHashes;
        }
        
        public IEnumerable<int> GetChildrenHashes(Owner player)
        {
            if (player == Owner.Me)
                return MyChildrenHashes;
            else
                return EnemyChildrenHashes;
        }

        public static TreeNode NewNodeWithChildren(TreeNode node, IEnumerable<int> childrenHashes, Owner player)
        {
            if(player == Owner.Me)
                return new TreeNode(node.Map, node.MoveList, childrenHashes, node.EnemyChildrenHashes);
            else
                return new TreeNode(node.Map, node.MoveList, node.MyChildrenHashes, childrenHashes);
        }

        public override int GetHashCode()
        {
            return Map.GetHashCode();
        }
    }
}
