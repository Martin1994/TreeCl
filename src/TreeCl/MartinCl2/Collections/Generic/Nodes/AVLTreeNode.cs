using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes
{
    /// <summary>
    /// Node of an AVL tree.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class AVLTreeNode<TKey, TValue> : AbstractAVLTreeNode<TKey, TValue, AVLTreeNode<TKey, TValue>>
    {
        public AVLTreeNode() : base()
        { }

        public AVLTreeNode(TKey key, TValue value) : base(key, value)
        { }
    }
}
