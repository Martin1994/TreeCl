using MartinCl2.Collections.Generic.Abstract;
using MartinCl2.Collections.Generic.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic
{
    /// <summary>
    /// Splay tree. After every operations, a related node will be splayed to the root.
    /// This implementation is not thread safe.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class SplayTree<TKey, TValue> : AbstractSplayTree<TKey, TValue, BinaryTreeNode<TKey, TValue>>
    {
        /// <summary>
        /// Create an empty binary search tree using the default comparer.
        /// </summary>
        public SplayTree() : base()
        { }

        /// <summary>
        /// Create an empty binary search tree using a given comparer.
        /// </summary>
        /// <param name="comparer">Key comparer</param>
        public SplayTree(IComparer<TKey> comparer) : base(comparer)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        public SplayTree(ICollection<KeyValuePair<TKey, TValue>> data) : base(data)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="comparer">Key comparer</param>
        public SplayTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer) : base(data, comparer)
        { }
    }
}
