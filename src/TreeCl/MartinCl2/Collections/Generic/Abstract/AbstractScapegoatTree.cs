using MartinCl2.Collections.Generic.Nodes;
using MartinCl2.Collections.Generic.Nodes.Abstract;
using MartinCl2.Collections.Generic.Nodes.Enumeration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Abstract
{
    /// <summary>
    /// Scapegoat tree. It keeps the whole tree alpha weight balanced by reconstructing the whole unbalanced sub-tree.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public abstract class AbstractScapegoatTree<TKey, TValue, TNode> : AbstractBinarySearchTree<TKey, TValue, TNode>
        where TNode : AbstractScapegoatTreeNode<TKey, TValue, TNode>, new()
    {
        private double _alpha = 0.7;
        public double Alpha
        {
            get { return _alpha; }
            protected set
            {
                if (value < 0.5 || value > 1.0)
                {
                    throw new ArgumentOutOfRangeException("alpha", "Alpha should be inside [0.5, 1.0]");
                }
                _alpha = value;
            }
        }

        /// <summary>
        /// Create an empty spaly tree using the default comparer.
        /// </summary>
        public AbstractScapegoatTree() : base()
        { }

        /// <summary>
        /// Create an empty splay tree using a given comparer.
        /// </summary>
        public AbstractScapegoatTree(IComparer<TKey> comparer) : base(comparer)
        { }

        /// <summary>
        /// Create a balanced splay tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        public AbstractScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data) : base(data)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        public AbstractScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer) : base(data, comparer)
        { }

        /// <summary>
        /// Create an empty spaly tree using the default comparer.
        /// <param name="alpha"></param>
        /// </summary>
        public AbstractScapegoatTree(int alpha) : base()
        {
            Alpha = alpha;
        }

        /// <summary>
        /// Create an empty splay tree using a given comparer.
        /// <param name="comparer"></param>
        /// <param name="alpha"></param>
        /// </summary>
        public AbstractScapegoatTree(IComparer<TKey> comparer, int alpha) : base(comparer)
        {
            Alpha = alpha;
        }

        /// <summary>
        /// Create a balanced splay tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="alpha"></param>
        public AbstractScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data, int alpha) : base(data)
        {
            // It is safe to set alpha after constructing the nodes because it promises a 0.5 weight balanced BST.
            _alpha = alpha;
        }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="comparer"></param>
        /// <param name="alpha"></param>
        public AbstractScapegoatTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer, int alpha) : base(data, comparer)
        {
            // It is safe to set alpha after constructing the nodes because it promises a 0.5 weight balanced BST.
            _alpha = alpha;
        }

        /// <summary>
        /// Construct a set of balanced tree nodes using the give data.
        /// </summary>
        /// <param name="nodes">Sorted nodes to construct</param>
        /// <param name="start">Starting offset of the nodes array</param>
        /// <param name="end">Ending offset of the nodes array</param>
        /// <returns>The root of the sub-tree contains the given nodes</returns>
        protected override TNode ConstructFromSortedNodes(TNode[] nodes, int start, int end)
        {
            TNode node = base.ConstructFromSortedNodes(nodes, start, end);
            if (node != null)
            {
                node.UpdateWeight();
            }
            return node;
        }

        /// <summary>
        /// Update weight and then rebuild a sub-tree if needed.
        /// </summary>
        /// <param name="ancestors"></param>
        protected void Rebuild(List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            for (int i = ancestors.Count - 1; i >= 0; i--)
            {
                ancestors[i].Node.UpdateWeight();
            }
            for (int i = 0; i < ancestors.Count; i++)
            {
                BinaryTreeNodeAncestor<TNode> ancestorRecord = ancestors[i];
                TNode ancestor = ancestorRecord.Node;
                double weightLimit = Alpha * ancestor.Weight;
                if (ancestor.LeftSubtreeWeight > weightLimit || ancestor.RightSubtreeWeight > weightLimit)
                {
                    TNode[] toRebuild = new TNode[ancestor.Weight];
                    BinaryTreeNodeEnumerator<TKey, TValue, TNode> enumerator = new BinaryTreeNodeEnumerator<TKey, TValue, TNode>(ancestor);
                    int j = 0;
                    while (enumerator.MoveNext())
                    {
                        toRebuild[j++] = enumerator.Current;
                    }
                    TNode newSubRoot = ConstructFromSortedNodes(toRebuild, 0, ancestor.Weight);
                    if (ancestorRecord.Position == BinaryTreeNodePosition.LeftChild)
                    {
                        ancestors[i - 1].Node.LeftChild = newSubRoot;
                    }
                    else if (ancestorRecord.Position == BinaryTreeNodePosition.RightChild)
                    {
                        ancestors[i - 1].Node.RightChild = newSubRoot;
                    }
                    else // ancestorRecord.Position == BinaryTreeNodePosition.Root
                    {
                        Root = newSubRoot;
                    }
                    break;
                }
            }
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

            // Rebalance
            Rebuild(ancestors);
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
            Debug.Assert(Root == ancestors[0].Node);
            int nodeIndex = ancestors.Count - 1;
            BinaryTreeNodeAncestor<TNode> nodeRecord = ancestors[nodeIndex];
            TNode node = nodeRecord.Node;
            TNode parent;
            if (ancestors.Count > 1)
            {
                parent = ancestors[ancestors.Count - 2].Node;
            }
            else
            {
                parent = null;
            }

            // Find the node that will fill the node's original position (a child of a parent or the root)
            TNode substitude = node.LeftChild;
            if (substitude == null)
            {
                // Left subtree doesn't exist
                substitude = node.RightChild;
            }
            else if (substitude.RightChild == null)
            {
                // Take the left leaf
                substitude.RightChild = node.RightChild;
            }
            else
            {
                ancestors.Add(new BinaryTreeNodeAncestor<TNode>(BinaryTreeNodePosition.LeftChild, substitude));
                // Take the largest left child
                while (substitude.RightChild.RightChild != null)
                {
                    substitude = substitude.RightChild;
                    ancestors.Add(new BinaryTreeNodeAncestor<TNode>(BinaryTreeNodePosition.RightChild, substitude));
                }
                TNode substitudeParent = substitude;
                substitude = substitude.RightChild;
                substitudeParent.RightChild = substitude.LeftChild;
                substitude.LeftChild = node.LeftChild;
                substitude.RightChild = node.RightChild;
            }
            // Remove substitude from ancestor list and replace node
            if (substitude == null)
            {
                ancestors.RemoveAt(ancestors.Count - 1);
            }
            else
            {
                ancestors[nodeIndex] = new BinaryTreeNodeAncestor<TNode>(ancestors[nodeIndex].Position, substitude);
            }

            // Update min and max and replace the removing node
            if (parent == null)
            {
                int minCompareResult = Compare(node.Key, MinNode.Key);
                int maxCompareResult = Compare(node.Key, MaxNode.Key);
                Debug.Assert(minCompareResult >= 0);
                Debug.Assert(maxCompareResult <= 0);
                if (minCompareResult == 0)
                {
                    MinNode = substitude;
                }
                if (maxCompareResult == 0)
                {
                    MaxNode = substitude;
                }
                Root = substitude;
            }
            else
            {
                if (nodeRecord.Position == BinaryTreeNodePosition.LeftChild)
                {
                    int compareResult = Compare(node.Key, MinNode.Key);
                    Debug.Assert(compareResult >= 0);
                    if (compareResult == 0)
                    {
                        if (substitude != null)
                        {
                            MinNode = substitude;
                        }
                        else
                        {
                            MinNode = parent;
                        }
                    }
                    parent.LeftChild = substitude;
                }
                else
                {
                    int compareResult = Compare(node.Key, MaxNode.Key);
                    Debug.Assert(compareResult <= 0);
                    if (compareResult == 0)
                    {
                        if (substitude != null)
                        {
                            MinNode = substitude;
                        }
                        else
                        {
                            MinNode = parent;
                        }
                    }
                    parent.RightChild = substitude;
                }
            }

            // Rebalance
            Rebuild(ancestors);
        }
    }
}
