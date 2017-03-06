using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes
{
    /// <summary>
    /// Node of an Scapegoat tree.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public sealed class ScapegoatTreeNode<TKey, TValue> : AbstractScapegoatTreeNode<TKey, TValue, ScapegoatTreeNode<TKey, TValue>>
    {
        public ScapegoatTreeNode() : base()
        { }

        public ScapegoatTreeNode(TKey key, TValue value) : base(key, value)
        { }
    }
}
