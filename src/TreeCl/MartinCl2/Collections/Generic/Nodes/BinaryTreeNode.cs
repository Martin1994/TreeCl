using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections.Generic;

namespace MartinCl2.Collections.Generic.Nodes
{
    /// <summary>
    /// Basic binary tree node with out any extra field.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class BinaryTreeNode<TKey, TValue>: AbstractBinaryTreeNode<TKey, TValue, BinaryTreeNode<TKey, TValue>>
    {
        public BinaryTreeNode() : base()
        { }

        public BinaryTreeNode(TKey key, TValue value) : base(key, value)
        { }
    }
}
