using MartinCl2.Collections.Generic.Abstract;
using MartinCl2.Collections.Generic.Nodes.Abstract;
using MartinCl2.Collections.Generic.Nodes.Enumeration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Enumeration
{
    /// <summary>
    /// Collection of nodes in a binary search tree
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinarySearchTreeNodeCollection<TKey, TValue, TNode> : ICollection<TNode>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>, new()
    {
        private readonly AbstractBinarySearchTree<TKey, TValue, TNode> _tree;
        private readonly TNode _root;
        private readonly TraversalOrder _order;
        private readonly bool _reversed;

        public BinarySearchTreeNodeCollection(AbstractBinarySearchTree<TKey, TValue, TNode> tree, TNode root, TraversalOrder order, bool reversed)
        {
            _tree = tree;
            _root = root;
            _order = order;
            _reversed = reversed;
        }

        /// <summary>
        /// Number of nodes
        /// </summary>
        public int Count
        {
            get { return _tree.Count; }
        }

        /// <summary>
        /// Wether this collectoin is read-only
        /// </summary>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="node"></param>
        public void Add(TNode node)
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
        /// Whether a node is in this collection
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Contains(TNode node)
        {
            return _tree.Contains(new KeyValuePair<TKey, TValue>(node.Key, node.Value));
        }

        /// <summary>
        /// Copy this collection to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(TNode[] array, int arrayIndex)
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
            foreach (TNode node in this)
            {
                array[i] = node;
                i++;
            }
        }

        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TNode> GetEnumerator()
        {
            // return new BinaryTreeNodeRangeEnumerator<TKey, TValue, TNode>(_root, default(TKey), default(TKey), null, true, true, true, true, _order, _reversed);
            return new BinaryTreeNodeEnumerator<TKey, TValue, TNode>(_root, _order, _reversed);
        }

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool Remove(TNode node)
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
