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
    /// Collection of values in a binary search tree
    /// </summary>
    public sealed class BinarySearchTreeValueCollection<TKey, TValue, TNode> : ICollection<TValue>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>, new()
    {
        private readonly AbstractBinarySearchTree<TKey, TValue, TNode> _tree;
        private readonly TNode _root;
        private readonly TraversalOrder _order;
        private readonly bool _reversed;
        private readonly ICollection<TNode> _nodes;
        private readonly IEnumerable<TValue> _values;

        public BinarySearchTreeValueCollection(AbstractBinarySearchTree<TKey, TValue, TNode> tree, TNode root, TraversalOrder order, bool reversed)
        {
            _tree = tree;
            _root = root;
            _order = order;
            _reversed = reversed;
            _nodes = new BinarySearchTreeNodeCollection<TKey, TValue, TNode>(tree, root, order, reversed);
            _values = _nodes.Select(node => node.Value);
        }

        /// <summary>
        /// Number of values
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
        /// <param name="value"></param>
        public void Add(TValue value)
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
        /// Whether a value is in this collection
        /// Warning: very inefficient!
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(TValue value)
        {
            return _nodes.Any(node => EqualityComparer<TValue>.Default.Equals(value, node.Value));
        }

        /// <summary>
        /// Copy this collection to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TValue[] array, int arrayIndex)
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
            foreach (TValue value in this)
            { 
                array[i] = value;
                i++;
            }
        }

        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TValue> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Remove(TValue value)
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
