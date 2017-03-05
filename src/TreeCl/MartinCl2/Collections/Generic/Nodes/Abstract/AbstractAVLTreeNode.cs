using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes.Abstract
{
    /// <summary>
    /// Abstract node of an AVL tree. It contains an extra height field.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public abstract class AbstractAVLTreeNode<TKey, TValue, TNode> : AbstractBinaryTreeNode<TKey, TValue, TNode>
        where TNode : AbstractAVLTreeNode<TKey, TValue, TNode>
    {
        private int _height = 1;

        public AbstractAVLTreeNode() : base()
        { }

        public AbstractAVLTreeNode(TKey key, TValue value) : base(key, value)
        { }

        /// <summary>
        /// Height of the sub-tree of which the root is this node
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        /// <summary>
        /// Height of the sub-tree of which the root is its left child
        /// </summary>
        public int LeftSubtreeHeight
        {
            get { return LeftChild == null ? 0 : LeftChild.Height; }
        }

        /// <summary>
        /// Height of the sub-tree of which the root is its right child
        /// </summary>
        public int RightSubtreeHeight
        {
            get { return RightChild == null ? 0 : RightChild.Height; }
        }

        /// <summary>
        /// Update the node's height by its children
        /// </summary>
        public void UpdateHeight()
        {
            Height = Math.Max(LeftSubtreeHeight, RightSubtreeHeight) + 1;
        }
    }
}
