using MartinCl2.Collections.Generic.Abstract;
using MartinCl2.Collections.Generic.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic
{
    /// <summary>
    /// Basic binary search tree without any balancing strategy.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class BinarySearchTree<TKey, TValue> : AbstractBinarySearchTree<TKey, TValue, BinaryTreeNode<TKey, TValue>>
    {
        /// <summary>
        /// Create an empty binary search tree using the default comparer.
        /// This implementation is not thread safe.
        /// </summary>
        public BinarySearchTree() : base()
        { }

        /// <summary>
        /// Create an empty binary search tree using a given comparer.
        /// </summary>
        /// <param name="comparer">Key comparer</param>
        public BinarySearchTree(IComparer<TKey> comparer) : base(comparer)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        public BinarySearchTree(ICollection<KeyValuePair<TKey, TValue>> data) : base(data)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="comparer">Key comparer</param>
        public BinarySearchTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer) : base(data, comparer)
        { }
    }
}
