using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MartinCl2.Collections.Generic.Abstract
{
    // TODO: Is this abstract layer actually necessary?

    /// <summary>
    /// An abstract tree representation
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public abstract class Tree<TKey, TValue, TNode> where TNode : TreeNode<TKey, TValue, TNode>, new()
    {
        /// <summary>
        /// Add a key value pair into the tree
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract void Add(TKey key, TValue value);
    }
}
