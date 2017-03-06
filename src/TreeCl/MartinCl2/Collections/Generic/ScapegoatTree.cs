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
    public sealed class ScapegoatTree<TKey, TValue> : AbstractScapegoatTree<TKey, TValue, ScapegoatTreeNode<TKey, TValue>>
    {

        /// <summary>
        /// Create an empty spaly tree using the default comparer.
        /// </summary>
        public ScapegoatTree() : base()
        { }

        /// <summary>
        /// Create an empty splay tree using a given comparer.
        /// </summary>
        public ScapegoatTree(IComparer<TKey> comparer) : base(comparer)
        { }

        /// <summary>
        /// Create a balanced splay tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        public ScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data) : base(data)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        public ScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer) : base(data, comparer)
        { }

        /// <summary>
        /// Create an empty spaly tree using the default comparer.
        /// <param name="alpha"></param>
        /// </summary>
        public ScapegoatTree(int alpha) : base(alpha)
        { }

        /// <summary>
        /// Create an empty splay tree using a given comparer.
        /// <param name="comparer"></param>
        /// <param name="alpha"></param>
        /// </summary>
        public ScapegoatTree(IComparer<TKey> comparer, int alpha) : base(comparer, alpha)
        { }

        /// <summary>
        /// Create a balanced splay tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alpha"></param>
        public ScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data, int alpha) : base(data, alpha)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        /// <param name="alpha"></param>
        public ScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer, int alpha) : base(data, comparer, alpha)
        { }
    }
}
