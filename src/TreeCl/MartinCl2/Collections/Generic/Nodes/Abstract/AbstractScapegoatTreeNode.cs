using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes.Abstract
{
    /// <summary>
    /// Abstract node of an scapegoat tree. It contains an extra weight field.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public abstract class AbstractScapegoatTreeNode<TKey, TValue, TNode> : AbstractBinaryTreeNode<TKey, TValue, TNode>
        where TNode : AbstractScapegoatTreeNode<TKey, TValue, TNode>
    {
        private int _weight = 1;

        public AbstractScapegoatTreeNode() : base()
        { }

        public AbstractScapegoatTreeNode(TKey key, TValue value) : base(key, value)
        { }

        /// <summary>
        /// Total number of nodes of the sub-tree of which the root this this node
        /// </summary>
        public int Weight
        {
            get { return _weight; }
            private set { _weight = value; }
        }

        /// <summary>
        /// Total number of nodes of the sub-tree of which the root is its left child
        /// </summary>
        public int LeftSubtreeWeight
        {
            get { return LeftChild == null ? 0 : LeftChild.Weight; }
        }

        /// <summary>
        /// Total number of nodes of the sub-tree of which the root is its right child
        /// </summary>
        public int RightSubtreeWeight
        {
            get { return RightChild == null ? 0 : RightChild.Weight; }
        }

        /// <summary>
        /// Update the node's weight by its children
        /// </summary>
        public void UpdateWeight()
        {
            Weight = LeftSubtreeWeight + RightSubtreeWeight + 1;
        }
    }
}
