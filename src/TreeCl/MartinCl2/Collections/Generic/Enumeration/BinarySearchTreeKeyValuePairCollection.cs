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
    /// Collection of key value pairs in a binary search tree
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinarySearchTreeKeyValuePairCollection<TKey, TValue, TNode> : ICollection<KeyValuePair<TKey, TValue>>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>, new()
    {
        private readonly AbstractBinarySearchTree<TKey, TValue, TNode> _tree;
        private readonly TNode _root;
        private readonly TraversalOrder _order;
        private readonly bool _reverse;
        private readonly ICollection<TNode> _node;

        public BinarySearchTreeKeyValuePairCollection(AbstractBinarySearchTree<TKey, TValue, TNode> tree, TNode root, TraversalOrder order, bool reverse)
        {
            _tree = tree;
            _root = root;
            _order = order;
            _reverse = reverse;
            _node = new BinarySearchTreeNodeCollection<TKey, TValue, TNode>(tree, root, order, reverse);
        }

        /// <summary>
        /// Number of items
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
        /// <param name="kvp"></param>
        public void Add(KeyValuePair<TKey, TValue> kvp)
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
        /// Whether an item is in this binary search tree
        /// </summary>
        /// <param name="kvp"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> kvp)
        {
            return _tree.Contains(kvp);
        }

        /// <summary>
        /// Copy this collection to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
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
            foreach (KeyValuePair<TKey, TValue> kvp in this)
            { 
                array[i] = kvp;
                i++;
            }
        }

        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _node.Select(node => new KeyValuePair<TKey, TValue>(node.Key, node.Value)).GetEnumerator();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="kvp"></param>
        /// <returns></returns>
        public bool Remove(KeyValuePair<TKey, TValue> kvp)
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
