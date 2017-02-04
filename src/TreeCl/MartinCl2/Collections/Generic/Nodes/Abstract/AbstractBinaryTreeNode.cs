using MartinCl2.Collections.Generic.Nodes.Enumeration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes.Abstract
{
    /// <summary>
    /// Abstract binary node class. Extend this class to add extra fields.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    [DebuggerDisplay("Key = {Key}, Value = {Value}")]
    public abstract class AbstractBinaryTreeNode<TKey, TValue, TNode> : TreeNode<TKey, TValue, TNode>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>
    {
        private TNode _leftChild = null;
        private TNode _rightChild = null;

        public AbstractBinaryTreeNode() : base()
        { }

        public AbstractBinaryTreeNode(TKey key, TValue value) : base(key, value)
        { }

        /// <summary>
        /// Left child node
        /// </summary>
        public TNode LeftChild
        {
            get { return _leftChild; }
            set { _leftChild = value; }
        }

        /// <summary>
        /// Right child node
        /// </summary>
        public TNode RightChild
        {
            get { return _rightChild; }
            set { _rightChild = value; }
        }

        /// <summary>
        /// Get all children in this tree node
        /// </summary>
        /// <returns></returns>
        protected override IReadOnlyList<TNode> GetChildren()
        {
            return new BinaryTreeNodeChildrenList<TKey, TValue, TNode>(this);
        }
    }
}
