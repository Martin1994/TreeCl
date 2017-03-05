using MartinCl2.Collections.Generic.Abstract;
using MartinCl2.Collections.Generic.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic
{
    /// <summary>
    /// AVL tree. It keeps the tree in O(n) height where n is the number of nodes in the tree by making the
    /// difference of height of left and right sub-trees no more than 1.
    /// This implementation is not thread safe.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class AVLTree<TKey, TValue> : AbstractAVLTree<TKey, TValue, AVLTreeNode<TKey, TValue>>
    {
        /// <summary>
        /// Create an empty binary search tree using the default comparer.
        /// </summary>
        public AVLTree() : base()
        { }

        /// <summary>
        /// Create an empty binary search tree using a given comparer.
        /// </summary>
        /// <param name="comparer">Key comparer</param>
        public AVLTree(IComparer<TKey> comparer) : base(comparer)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        public AVLTree(ICollection<KeyValuePair<TKey, TValue>> data) : base(data)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="comparer">Key comparer</param>
        public AVLTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer) : base(data, comparer)
        { }
    }
}
