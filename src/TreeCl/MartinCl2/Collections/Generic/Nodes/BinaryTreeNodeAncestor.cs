using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes
{
    /// <summary>
    /// Represent the position of a node in an ancestor list
    /// </summary>
    public enum BinaryTreeNodePosition
    {
        /// <summary>
        /// This node is a root
        /// </summary>
        Root = 0,
        /// <summary>
        /// This node is the left child of its parent
        /// </summary>
        LeftChild,
        /// <summary>
        /// This node is the right child of its parent
        /// </summary>
        RightChild
    }

    /// <summary>
    /// Represent a record in an ancestor list
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public struct BinaryTreeNodeAncestor<TNode>
    {
        /// <summary>
        /// Node position
        /// </summary>
        public readonly BinaryTreeNodePosition Position;
        /// <summary>
        /// Node itself
        /// </summary>
        public readonly TNode Node;

        public BinaryTreeNodeAncestor(BinaryTreeNodePosition position, TNode node)
        {
            Position = position;
            Node = node;
        }
    }
    
}
