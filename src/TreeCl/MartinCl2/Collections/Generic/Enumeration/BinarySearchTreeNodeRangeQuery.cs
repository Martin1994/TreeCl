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
    /// The result of a range query to a binary search tree
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinarySearchTreeNodeRangeQuery<TKey, TValue, TNode> : IEnumerable<TNode>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>, new()
    {
        private readonly AbstractBinarySearchTree<TKey, TValue, TNode> _tree;
        private readonly TNode _root;
        private readonly TKey _from;
        private readonly TKey _to;
        private readonly TraversalOrder _order;
        private readonly bool _reversed;
        private readonly RangeQueryFlag _flag;
        private readonly IComparer<TKey> _comparer;

        public BinarySearchTreeNodeRangeQuery(TNode root, TKey from, TKey to, IComparer<TKey> comparer, RangeQueryFlag flag = RangeQueryFlag.IncludeLeft | RangeQueryFlag.IncludeRight, TraversalOrder order = TraversalOrder.InOrder, bool reversed = false)
        {
            _root = root;
            _from = from;
            _to = to;
            _order = order;
            _reversed = reversed;
            _flag = flag;
            _comparer = comparer;
        }

        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TNode> GetEnumerator()
        {
            return new BinaryTreeNodeRangeEnumerator<TKey, TValue, TNode>(_root, _from, _to, _comparer, _flag, _order, _reversed);
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
