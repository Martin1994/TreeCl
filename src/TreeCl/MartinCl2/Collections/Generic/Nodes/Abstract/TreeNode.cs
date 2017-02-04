using System;
using System.Collections.Generic;

namespace MartinCl2.Collections.Generic.Nodes.Abstract
{
    /// <summary>
    /// Abstract tree node.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public abstract class TreeNode<TKey, TValue, TNode> where TNode : TreeNode<TKey, TValue, TNode>
    {
        private TKey _key;
        private TValue _value;

        public TreeNode()
        { }

        public TreeNode(TKey key, TValue value) {
            _key = key;
            _value = value;
        }

        /// <summary>
        /// Node key
        /// </summary>
        public TKey Key
        {
            get {  return _key; }
            set { _key = value; }
        }

        /// <summary>
        /// Node value
        /// </summary>
        public TValue Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// Get all children in this tree node
        /// </summary>
        /// <returns></returns>
        protected abstract IReadOnlyList<TNode> GetChildren();
    }
}
