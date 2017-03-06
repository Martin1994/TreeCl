using MartinCl2.Collections.Generic.Nodes;
using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Abstract
{
    /// <summary>
    /// Splay tree. After every operations, a related node will be splayed to the root.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public abstract class AbstractSplayTree<TKey, TValue, TNode> : AbstractBinarySearchTree<TKey, TValue, TNode>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>, new()
    {
        /// <summary>
        /// Create an empty spaly tree using the default comparer.
        /// </summary>
        public AbstractSplayTree() : base()
        { }

        /// <summary>
        /// Create an empty splay tree using a given comparer.
        /// <param name="comparer"></param>
        /// </summary>
        public AbstractSplayTree(IComparer<TKey> comparer) : base(comparer)
        { }

        /// <summary>
        /// Create a balanced splay tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        public AbstractSplayTree(ICollection<KeyValuePair<TKey, TValue>> data) : base(data)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        public AbstractSplayTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer) : base(data, comparer)
        { }

        /// <summary>
        /// Splay a node to the root from bottom to up.
        /// </summary>
        /// <param name="ancestors">All ancestors of this node. Should be non-empty. The first element should be the root, and the last element is the node itself.</param>
        protected void Splay(List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            Debug.Assert(ancestors != null);
            Debug.Assert(ancestors.Count > 0);
            Debug.Assert(ancestors[0].Node == Root);
            int currentDepth = ancestors.Count - 1;
            while (currentDepth > 0)
            {
                if (currentDepth == 1)
                {
                    // Only have two layers. Zig step.
                    if (ancestors[1].Position == BinaryTreeNodePosition.LeftChild)
                    {
                        Root = RotateRight(Root);
                    }
                    else
                    {
                        Root = RotateLeft(Root);
                    }
                    Debug.Assert(Root == ancestors[ancestors.Count - 1].Node);
                    currentDepth -= 1;
                }
                else
                {
                    // More than two layers. Zig-zig step or zig-zag step.
                    TNode rotateResult;
                    if (ancestors[currentDepth].Position == BinaryTreeNodePosition.LeftChild)
                    {
                        if (ancestors[currentDepth - 1].Position == BinaryTreeNodePosition.LeftChild)
                        {
                            rotateResult = RotateRightRight(ancestors[currentDepth - 2].Node);
                        }
                        else
                        {
                            rotateResult = RotateRightLeft(ancestors[currentDepth - 2].Node);
                        }
                    }
                    else
                    {
                        if (ancestors[currentDepth - 1].Position == BinaryTreeNodePosition.LeftChild)
                        {
                            rotateResult = RotateLeftRight(ancestors[currentDepth - 2].Node);
                        }
                        else
                        {
                            rotateResult = RotateLeftLeft(ancestors[currentDepth - 2].Node);
                        }
                    }
                    // Check where to place the rotated sub-tree
                    Debug.Assert(rotateResult == ancestors[ancestors.Count - 1].Node);
                    if (currentDepth == 2)
                    {
                        Root = rotateResult;
                    }
                    else if (ancestors[currentDepth - 2].Position == BinaryTreeNodePosition.LeftChild)
                    {
                        ancestors[currentDepth - 3].Node.LeftChild = rotateResult;
                    }
                    else
                    {
                        ancestors[currentDepth - 3].Node.RightChild = rotateResult;
                    }
                    currentDepth -= 2;
                }
            }
            Debug.Assert(Root == ancestors[ancestors.Count - 1].Node);
        }

        /// <summary>
        /// Add a node to a parent and splay it to the root.
        /// </summary>
        /// <param name="compareResult"></param>
        /// <param name="ancestors">All ancestors of this node. Should be non-empty. The first element should be the root, and the last element is the node itself.</param>
        protected override void AddToParent(int compareResult, TNode node, List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            Debug.Assert(ancestors != null);
            Debug.Assert(ancestors.Count > 0);
            Debug.Assert(ancestors[0].Node == Root);
            base.AddToParent(compareResult, node, ancestors);
            ancestors.Add(new BinaryTreeNodeAncestor<TNode>(
                compareResult < 0 ? BinaryTreeNodePosition.LeftChild : BinaryTreeNodePosition.RightChild,
                node
            ));
            Splay(ancestors);
        }

        /// <summary>
        /// Every time a node is explicitly accessed, splay it to the root
        /// </summary>
        /// <param name="ancestors">All ancestors of this node. Should be non-empty. The first element should be the root, and the last element is the node itself.</param>
        protected override void RecordExplicitlyAccessedNode(List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            Debug.Assert(ancestors != null);
            Debug.Assert(ancestors.Count > 0);
            Debug.Assert(ancestors[0].Node == Root);
            Splay(ancestors);
        }

        /// <summary>
        /// Remove a node from its parent then splay its parent to the root.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ancestors">All ancestors of this node. Should be non-empty. The first element should be the root, and the last element is the node itself.</param>
        protected override void RemoveFromParent(List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            Debug.Assert(ancestors != null);
            Debug.Assert(ancestors.Count > 0);
            Debug.Assert(ancestors[0].Node == Root);
            base.RemoveFromParent(ancestors);
            ancestors.RemoveAt(ancestors.Count - 1);
            if (ancestors.Count > 0)
            {
                Splay(ancestors);
            }
        }
    }
}
