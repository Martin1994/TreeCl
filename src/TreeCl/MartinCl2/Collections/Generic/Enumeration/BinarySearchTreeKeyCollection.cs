using MartinCl2.Collections.Generic.Abstract;
using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Enumeration
{
    /// <summary>
    /// Collection of keys in a binary search tree
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinarySearchTreeKeyCollection<TKey, TValue, TNode> : ICollection<TKey>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>, new()
    {
        private readonly AbstractBinarySearchTree<TKey, TValue, TNode> _tree;
        private readonly TNode _root;
        private readonly TraversalOrder _order;
        private readonly bool _reversed;
        private readonly ICollection<TNode> _nodes;
        private readonly IEnumerable<TKey> _keys;

        public BinarySearchTreeKeyCollection(AbstractBinarySearchTree<TKey, TValue, TNode> tree, TNode root, TraversalOrder order, bool reversed)
        {
            _tree = tree;
            _root = root;
            _order = order;
            _reversed = reversed;
            _nodes = new BinarySearchTreeNodeCollection<TKey, TValue, TNode>(tree, root, order, reversed);
            _keys = _nodes.Select(node => node.Key);
        }

        /// <summary>
        /// Number of keys
        /// </summary>
        public int Count
        {
            get { return _tree.Count; }
        }

        /// <summary>
        /// Whether this collection is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="key"></param>
        public void Add(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Whether a key is in this collection
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(TKey key)
        {
            return _tree.ContainsKey(key);
        }

        /// <summary>
        /// Copy this collection to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TKey[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            if (arrayIndex + Count > array.Length)
            {
                throw new ArgumentException("The number of elements is greater than the available number of elements from index to the end of the destination array.", "arrayIndex");
            }
            int i = arrayIndex;
            foreach (TKey key in this)
            { 
                array[i] = key;
                i++;
            }
        }

        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TKey> GetEnumerator()
        {
            return _keys.GetEnumerator();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
