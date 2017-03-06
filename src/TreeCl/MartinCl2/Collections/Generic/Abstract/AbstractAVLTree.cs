using MartinCl2.Collections.Generic.Nodes.Abstract;
using System.Collections.Generic;
using System.Diagnostics;
using MartinCl2.Collections.Generic.Nodes;

namespace MartinCl2.Collections.Generic.Abstract
{
    /// <summary>
    /// Abstract AVL tree.
    /// Since every node in an AVL tree satisfies that the difference between height of left and right
    /// sub-tree is never greater than 1, it guarantees an O(log n) height.
    /// This implementation records the actual tree height instead of balance factor (i.e. the difference
    /// between the sub-trees' height: -1, 0 or 1) as an AVL tree implementation usually does.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public class AbstractAVLTree<TKey, TValue, TNode> : AbstractBinarySearchTree<TKey, TValue, TNode>
        where TNode : AbstractAVLTreeNode<TKey, TValue, TNode>, new()
    {
        public AbstractAVLTree() : base()
        { }

        public AbstractAVLTree(IComparer<TKey> comparer) : base(comparer)
        { }

        public AbstractAVLTree(ICollection<KeyValuePair<TKey, TValue>>data) : base(data)
        { }

        public AbstractAVLTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer) : base (data, comparer)
        { }

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
                node.UpdateHeight();
            }
            return node;
        }

        /// <summary>
        /// Add a node to its parent.
        /// </summary>
        /// <param name="compareResult"></param>
        /// <param name="node"></param>
        /// <param name="ancestors"></param>
        protected override void AddToParent(int compareResult, TNode node, List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            Debug.Assert(ancestors.Count > 0);
            base.AddToParent(compareResult, node, ancestors);
            ancestors[ancestors.Count - 1].Node.UpdateHeight();
            ancestors.Add(new BinaryTreeNodeAncestor<TNode>(
                compareResult < 0 ? BinaryTreeNodePosition.LeftChild : BinaryTreeNodePosition.RightChild,
                node
            ));

            // Balance
            for (int i = ancestors.Count - 1; i >= 2; i--)
            {
                BinaryTreeNodeAncestor<TNode> ancestorRecord = ancestors[i];
                TNode ancestor = ancestorRecord.Node;
                BinaryTreeNodeAncestor<TNode> ancestorParentRecord = ancestors[i - 1];
                TNode ancestorParent = ancestorParentRecord.Node;
                BinaryTreeNodeAncestor<TNode> ancestorGrandParentRecord = ancestors[i - 2];
                TNode ancestorGrandParent = ancestorGrandParentRecord.Node;
                int balanceFactor = ancestorGrandParent.RightSubtreeHeight - ancestorGrandParent.LeftSubtreeHeight;
                int oldHeight = ancestorGrandParent.Height;
                TNode newSubRoot;
                if (balanceFactor < -1)
                {
                    Debug.Assert(ancestorParentRecord.Position == BinaryTreeNodePosition.LeftChild);
                    if (ancestorRecord.Position == BinaryTreeNodePosition.LeftChild)
                    {
                        newSubRoot = RotateRight(ancestorGrandParent);
                        ancestorGrandParent.UpdateHeight();
                        ancestorParent.UpdateHeight();
                    }
                    else // ancestorRecord.Position == BinaryTreeNodePosition.RightChild
                    {
                        newSubRoot = RotateLeftRight(ancestorGrandParent);
                        ancestorGrandParent.UpdateHeight();
                        ancestorParent.UpdateHeight();
                        ancestor.UpdateHeight();
                    }
                }
                else if (balanceFactor > 1)
                {
                    Debug.Assert(ancestorParentRecord.Position == BinaryTreeNodePosition.RightChild);
                    if (ancestorRecord.Position == BinaryTreeNodePosition.LeftChild)
                    {
                        newSubRoot = RotateRightLeft(ancestorGrandParent);
                        ancestorGrandParent.UpdateHeight();
                        ancestorParent.UpdateHeight();
                        ancestor.UpdateHeight();
                    }
                    else // ancestorRecord.Position == BinaryTreeNodePosition.RightChild
                    {
                        newSubRoot = RotateLeft(ancestorGrandParent);
                        ancestorGrandParent.UpdateHeight();
                        ancestorParent.UpdateHeight();
                    }
                }
                else
                {
                    ancestorGrandParent.UpdateHeight();
                    newSubRoot = ancestorGrandParent;
                }
                if (newSubRoot != ancestorGrandParent)
                {
                    if (ancestorGrandParentRecord.Position == BinaryTreeNodePosition.LeftChild)
                    {
                        Debug.Assert(i >= 3);
                        ancestors[i - 3].Node.LeftChild = newSubRoot;
                    }
                    else if (ancestorGrandParentRecord.Position == BinaryTreeNodePosition.RightChild)
                    {
                        Debug.Assert(i >= 3);
                        ancestors[i - 3].Node.RightChild = newSubRoot;
                    }
                    else // ancestorGrandParentRecord.Position == BinaryTreeNodePosition.Root
                    {
                        Root = newSubRoot;
                    }
                    // There will be at most one rotation
                    break;
                }
                if (newSubRoot.Height == oldHeight)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Remove a node from this tree.
        /// Override this method to do balancing but remember to update min and max node as well.
        /// </summary>
        /// <param name="ancestors">All ancestors of this node. Should be non-empty. The first element is the root of the tree and the last element is the node to be removed. If the node itself is not the root, the second last node is its parent.</param>
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

            // Do the balancing.
            // Now the ancestor list contains nodes from the root all the way down to the parent of the subsitude
            // Unlike balacing while inserting, here we don't have the information that which sub-tree is higher which
            // is useful to determine whether to use single rotation or double rotation.
            // Update the substitude node first in case we won't reach it.
            if (substitude != null)
            {
                substitude.UpdateHeight();
            }
            for (int i = ancestors.Count - 1; i >= 0; i--)
            {
                BinaryTreeNodeAncestor<TNode> ancestorRecord = ancestors[i];
                TNode ancestor = ancestorRecord.Node;
                int balanceFactor = ancestor.RightSubtreeHeight - ancestor.LeftSubtreeHeight;
                int oldHeight = ancestor.Height;
                TNode newSubRoot;
                if (balanceFactor < -1)
                {
                    Debug.Assert(ancestor.LeftChild != null);
                    TNode ancestorChild = ancestor.LeftChild;
                    if (ancestorChild.LeftSubtreeHeight >= ancestorChild.RightSubtreeHeight)
                    {
                        newSubRoot = RotateRight(ancestor);
                        ancestor.UpdateHeight();
                        ancestorChild.UpdateHeight();
                    }
                    else
                    {
                        TNode ancestorGrandChild = ancestorChild.RightChild;
                        newSubRoot = RotateLeftRight(ancestor);
                        ancestor.UpdateHeight();
                        ancestorChild.UpdateHeight();
                        ancestorGrandChild.UpdateHeight();
                    }
                }
                else if (balanceFactor > 1)
                {
                    Debug.Assert(ancestor.RightChild != null);
                    TNode ancestorChild = ancestor.RightChild;
                    if (ancestorChild.LeftSubtreeHeight <= ancestorChild.RightSubtreeHeight)
                    {
                        newSubRoot = RotateLeft(ancestor);
                        ancestor.UpdateHeight();
                        ancestorChild.UpdateHeight();
                    }
                    else
                    {
                        TNode ancestorGrandChild = ancestorChild.LeftChild;
                        newSubRoot = RotateRightLeft(ancestor);
                        ancestor.UpdateHeight();
                        ancestorChild.UpdateHeight();
                        ancestorGrandChild.UpdateHeight();
                    }
                }
                else
                {
                    ancestor.UpdateHeight();
                    newSubRoot = ancestor;
                }
                if (newSubRoot != ancestor)
                {
                    if (ancestorRecord.Position == BinaryTreeNodePosition.LeftChild)
                    {
                        Debug.Assert(i >= 1);
                        ancestors[i - 1].Node.LeftChild = newSubRoot;
                    }
                    else if (ancestorRecord.Position == BinaryTreeNodePosition.RightChild)
                    {
                        Debug.Assert(i >= 1);
                        ancestors[i - 1].Node.RightChild = newSubRoot;
                    }
                    else // ancestorRecord.Position == BinaryTreeNodePosition.Root
                    {
                        Root = newSubRoot;
                    }
                }
                if (newSubRoot.Height == oldHeight)
                {
                    break;
                }
            }
        }

    }
}
