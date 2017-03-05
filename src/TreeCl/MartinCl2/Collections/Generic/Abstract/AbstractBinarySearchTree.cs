using MartinCl2.Collections.Generic.Enumeration;
using MartinCl2.Collections.Generic.Nodes;
using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Abstract
{
    /// <summary>
    /// Abstract binary search tree without any balancing strategy.
    /// This class is supposed to be extended so that balancing strategies can be added.
    /// This implementation is not thread safe.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode">Tree node type used in this binary search tree</typeparam>
    [DebuggerDisplay("Count = {Count}, Min = {Min}, Max = {Max}")]
    public abstract class AbstractBinarySearchTree<TKey, TValue, TNode> : IBinarySearchTree<TKey, TValue>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>, new()
    {
        /// <summary>
        /// Type of key to return if the given key is not in the tree while doing a search.
        /// </summary>
        protected enum TryGetSide
        {
            /// <summary>
            /// Must find the exact key.
            /// </summary>
            Exact = 0,

            /// <summary>
            /// Find the closest smaller key (inclusive).
            /// </summary>
            Left = -1,

            /// <summary>
            /// Find the closest bigger key (inclusive).
            /// </summary>
            Right = 1,

            /// <summary>
            /// Find the deepest ancestor.
            /// The output node would be the parent if the given key were going to be inserted into the tree.
            /// <see>GeneralTryGetNode</see>
            /// </summary>
            Parent = 2
        }

        /// <summary>
        /// Action to do when trying to add an existing key into the tree.
        /// </summary>
        private enum AddStrategy
        {
            /// <summary>
            /// Keep the original value if key is duplicate.
            /// </summary>
            Keep = 0,

            /// <summary>
            /// Override the original value if key is duplicate.
            /// </summary>
            Override = 1
        }

        private TNode _root = null;
        /// <summary>
        /// Root of the binary search tree.
        /// </summary>
        protected TNode Root
        {
            get { return _root; }
            set { _root = value; }
        }

        private readonly IComparer<TKey> _comparer;

        protected IComparer<TKey> Comparer
        {
            get { return _comparer; }
        }

        /// <summary>
        /// Key compare function.
        /// </summary>
        protected int Compare(TKey lhs, TKey rhs)
        {
            return _comparer.Compare(lhs, rhs);
        }

        /// <summary>
        /// Value compare function.
        /// Only used in IDictionary&lt;TKey, TValue&gt; implementations.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private static bool IsValueEqual(TValue lhs, TValue rhs)
        {
            return EqualityComparer<TValue>.Default.Equals(lhs, rhs);
        }

        /// <summary>
        /// Create an empty binary search tree using the default comparer.
        /// </summary>
        public AbstractBinarySearchTree()
        {
            _comparer = Comparer<TKey>.Default;
        }

        /// <summary>
        /// Create an empty binary search tree using a given comparer.
        /// </summary>
        public AbstractBinarySearchTree(IComparer<TKey> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            _comparer = comparer;
        }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using the default comparer.
        /// </summary>
        /// <param name="data"></param>
        public AbstractBinarySearchTree(ICollection<KeyValuePair<TKey, TValue>> data) : this(data, Comparer<TKey>.Default)
        { }

        /// <summary>
        /// Create a balanced binary search tree with given key value pairs using a given comparer.
        /// </summary>
        /// <param name="data"></param>
        public AbstractBinarySearchTree(ICollection<KeyValuePair<TKey, TValue>> data, IComparer<TKey> comparer)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            _comparer = comparer;
            TNode[] sortedNodes = data.Select(kvp => new TNode()
            {
                Key = kvp.Key,
                Value = kvp.Value
            }).ToArray();
            Array.Sort(sortedNodes, (lhs, rhs) =>
            {
                return Compare(lhs.Key, rhs.Key);
            });
            if (sortedNodes.Length > 0)
            {
                MinNode = sortedNodes[0];
                MaxNode = sortedNodes[sortedNodes.Length - 1];
            }
            Root = ConstructFromSortedNodes(sortedNodes, 0, sortedNodes.Length);
            Count = data.Count();
        }

        /// <summary>
        /// Get the value by the given key or set the value to this key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual TValue this[TKey key]
        {
            get
            {
                List<BinaryTreeNodeAncestor<TNode>> ancestors;
                TNode node = GeneralTryGetNode(key, TryGetSide.Exact, out ancestors);
                if (node == null)
                {
                    throw new KeyNotFoundException("This key doesn't exist.");
                }
                RecordExplicitlyAccessedNode(ancestors);
                return node.Value;
            }

            set
            {
                GeneralAdd(key, value, AddStrategy.Override);
            }
        }

        private TNode _maxNode = null;
        /// <summary>
        /// The node with maximum key in the tree
        /// </summary>
        protected TNode MaxNode
        {
            get { return _maxNode; }
            set { _maxNode = value; }
        }
        /// <summary>
        /// The maximum key and its value in the tree
        /// </summary>
        public KeyValuePair<TKey, TValue> Max
        {
            get
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException();
                }
                return new KeyValuePair<TKey, TValue>(MaxNode.Key, MaxNode.Value);
            }
        }

        private TNode _minNode = null;
        /// <summary>
        /// The node with minimum key in the tree
        /// </summary>
        protected TNode MinNode
        {
            get { return _minNode; }
            set { _minNode = value; }
        }
        /// <summary>
        /// The minimum key and its value in the tree
        /// </summary>
        public KeyValuePair<TKey, TValue> Min
        {
            get
            {
                if (Count == 0)
                {
                    throw new InvalidOperationException();
                }
                return new KeyValuePair<TKey, TValue>(MinNode.Key, MinNode.Value);
            }
        }

        private int _count = 0;
        /// <summary>
        /// Number of values in this tree.
        /// </summary>
        public int Count
        {
            get { return _count; }
            private set { _count = value; }
        }

        /// <summary>
        /// Whether this tree is read only
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Get all keys in increasing order
        /// </summary>
        public ICollection<TKey> Keys
        {
            get { return GetKeys(); }
        }

        /// <summary>
        /// Get all values in increasing key order
        /// </summary>
        public ICollection<TValue> Values
        {
            get { return GetValues(); }
        }

        /// <summary>
        /// Get all nodes in increasing key order
        /// </summary>
        protected IEnumerable<TNode> Nodes
        {
            get { return GetNodes(); }
        }

        /// <summary>
        /// Get all keys.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        public ICollection<TKey> GetKeys(TraversalOrder order = TraversalOrder.InOrder, bool reversed = false)
        {
            return new BinarySearchTreeKeyCollection<TKey, TValue, TNode>(this, Root, order, reversed);
        }

        /// <summary>
        /// Get all values.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        public ICollection<TValue> GetValues(TraversalOrder order = TraversalOrder.InOrder, bool reversed = false)
        {
            return new BinarySearchTreeValueCollection<TKey, TValue, TNode>(this, Root, order, reversed);
        }

        /// <summary>
        /// Get all key value pairs.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        public ICollection<KeyValuePair<TKey, TValue>> GetKeyValuePairs(TraversalOrder order = TraversalOrder.InOrder, bool reversed = false)
        {
            return new BinarySearchTreeKeyValuePairCollection<TKey, TValue, TNode>(this, Root, order, reversed);
        }

        /// <summary>
        /// Get all nodes
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reverse"></param>
        /// <returns></returns>
        protected IEnumerable<TNode> GetNodes(TraversalOrder order = TraversalOrder.InOrder, bool reverse = false)
        {
            return new BinarySearchTreeNodeCollection<TKey, TValue, TNode>(this, Root, order, reverse);
        }

        /// <summary>
        /// Construct a set of balanced tree nodes using the give data.
        /// This method construct a sub-tree of a sub-range of the given node array and then
        /// return the root of this sub-tree.
        /// Min and max must be properly set before calling this method.
        /// Override this method to update extra node fields.
        /// </summary>
        /// <param name="nodes">Sorted nodes to construct</param>
        /// <param name="start">Starting offset of the nodes array</param>
        /// <param name="end">Ending offset of the nodes array</param>
        /// <returns>The root of the sub-tree contains the given nodes</returns>
        protected virtual TNode ConstructFromSortedNodes(TNode[] nodes, int start, int end)
        {
            Debug.Assert(start >= 0);
            Debug.Assert(end >= start);
            Debug.Assert(nodes.Length >= end);
            Debug.Assert(nodes.Length == 0 || MinNode != null && MaxNode != null);
            if (end - start > 0)
            {
                int mid = start + (end - start) / 2;
                TNode node = nodes[mid];
                node.LeftChild = ConstructFromSortedNodes(nodes, start, mid);
                node.RightChild = ConstructFromSortedNodes(nodes, mid + 1, end);
                Debug.Assert(node.LeftChild == null || Compare(node.LeftChild.Key, node.Key) <= 0);
                Debug.Assert(node.RightChild == null || Compare(node.RightChild.Key, node.Key) >= 0);
                return node;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Add a key value pair
        /// </summary>
        /// <param name="item"></param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Add a key with a value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Add(TKey key, TValue value)
        {
            bool success = GeneralAdd(key, value, AddStrategy.Keep);
            if (!success)
            {
                throw new ArgumentException("An element with the same key already exists.");
            }
        }

        /// <summary>
        /// Add a key with a value using given conflict strategy.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="strategy">Actions to do when the given key exists in this tree</param>
        /// <returns>Success</returns>
        private bool GeneralAdd(TKey key, TValue value, AddStrategy strategy)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode parent = GetParentBeforeInsert(key, out ancestors);
            TNode node = new TNode()
            {
                Key = key,
                Value = value
            };
            if (parent == null)
            {
                Root = node;
                MaxNode = node;
                MinNode = node;
                Count++;
                return true;
            }
            int compareResult = Compare(key, parent.Key);
            if (compareResult == 0)
            {
                if (strategy == AddStrategy.Keep)
                {
                    return false;
                }
                else
                {
                    parent.Value = value;
                    return true;
                }
            }
            else
            {
                Count++;
                if (compareResult < 0)
                {
                    if (Compare(node.Key, MinNode.Key) < 0)
                    {
                        MinNode = node;
                    }
                }
                else
                {
                    if (Compare(node.Key, MaxNode.Key) > 0)
                    {
                        MaxNode = node;
                    }
                }
                AddToParent(compareResult, node, ancestors);
                return true;
            }
        }

        /// <summary>
        /// Add a node to its parent. Override this method to do balancing but remember updating min and max node as well.
        /// </summary>
        /// <param name="compareResult">Compare result between this node and its parent</param>
        /// <param name="node">The inserting node</param>
        /// <param name="ancestors">The ancestors path wherethe first element is the root and the last element is the parent</param>
        protected virtual void AddToParent(int compareResult, TNode node, List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            Debug.Assert(compareResult != 0);
            Debug.Assert(node != null);
            Debug.Assert(ancestors != null);
            Debug.Assert(ancestors.Count > 0);
            Debug.Assert(ancestors[0].Node == Root);
            BinaryTreeNodeAncestor<TNode> ancestorRecord = ancestors[ancestors.Count - 1];
            TNode parent = ancestorRecord.Node;
            if (compareResult < 0)
            {
                parent.LeftChild = node;
            }
            else
            {
                parent.RightChild = node;
            }
        }

        /// <summary>
        /// Do some action when a node is explicitly being got. Override this method to record extra information or adjust the tree.
        /// Intentionally left blank. No specific action to do in the default implementation.
        /// </summary>
        /// <param name="ancestors">Ancestors of the node. Should be non-empty. The first element is the root, and the last element is the node itself.</param>
        protected virtual void RecordExplicitlyAccessedNode(List<BinaryTreeNodeAncestor<TNode>> ancestors)
        { }

        /// <summary>
        /// Get the value with the largest key that is less than or equal to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetLeftClosestValue(TKey key)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Left, out ancestors);
            if (node == null)
            {
                throw new KeyNotFoundException("This key is less than any existing keys.");
            }
            RecordExplicitlyAccessedNode(ancestors);
            return node.Value;
        }

        /// <summary>
        /// Get the value with the smallest key that is greater than or equal to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue GetRightClosestValue(TKey key)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Right, out ancestors);
            if (node == null)
            {
                throw new KeyNotFoundException("This key is larger than any existing keys.");
            }
            RecordExplicitlyAccessedNode(ancestors);
            return node.Value;
        }

        /// <summary>
        /// Get the value by the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Success</returns>
        public bool TryGetValue(TKey key, out TValue resultValue)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Exact, out ancestors);
            if (node == null)
            {
                resultValue = default(TValue);
                return false;
            }
            else
            {
                RecordExplicitlyAccessedNode(ancestors);
                resultValue = node.Value;
                return true;
            }
        }

        /// <summary>
        /// Get the value with the largest key that is less than or equal to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>success</returns>
        public bool TryGetLeftClosestValue(TKey key, out TValue resultValue)
        {

            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Left, out ancestors);
            if (node == null)
            {
                resultValue = default(TValue);
                return false;
            }
            else
            {
                RecordExplicitlyAccessedNode(ancestors);
                resultValue = node.Value;
                return true;
            }
        }

        /// <summary>
        /// With its value, get the largest key that is less than or equal to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>success</returns>
        public bool TryGetLeftClosestValue(TKey key, out TKey resultKey, out TValue resultValue)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Left, out ancestors);
            if (node == null)
            {
                resultKey = default(TKey);
                resultValue = default(TValue);
                return false;
            }
            else
            {
                RecordExplicitlyAccessedNode(ancestors);
                resultKey = node.Key;
                resultValue = node.Value;
                return true;
            }
        }

        /// <summary>
        /// Get the value with the smallest key that is greater than or equal to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>success</returns>
        public bool TryGetRightClosestValue(TKey key, out TValue resultValue)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Right, out ancestors);
            if (node == null)
            {
                resultValue = default(TValue);
                return false;
            }
            else
            {
                RecordExplicitlyAccessedNode(ancestors);
                resultValue = node.Value;
                return true;
            }
        }

        /// <summary>
        /// With its value, get the smallest key that is greater than or equal to the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>success</returns>
        public bool TryGetRightClosestValue(TKey key, out TKey resultKey, out TValue resultValue)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Right, out ancestors);
            if (node == null)
            {
                resultKey = default(TKey);
                resultValue = default(TValue);
                return false;
            }
            else
            {
                RecordExplicitlyAccessedNode(ancestors);
                resultKey = node.Key;
                resultValue = node.Value;
                return true;
            }
        }

        /// <summary>
        /// Get the parent node while inserting a new node.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ancestors"></param>
        /// <returns></returns>
        protected TNode GetParentBeforeInsert(TKey key, out List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            return GeneralTryGetNode(key, TryGetSide.Parent, out ancestors);
        }

        /// <summary>
        /// Get a node by a key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="side">The prefered node side when the given key doesn't exist</param>
        /// <param name="ancestors">All ancestors along the searching path including the return value</param>
        /// <returns>Success</returns>
        protected TNode GeneralTryGetNode(TKey key, TryGetSide side, out List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            // The current node doing the iteration
            TNode current = Root;
            // The most right ancestor that is left to the current node
            TNode leftAncestor = null;
            // The most left ancestor that is right to the current node
            TNode rightAncestor = null;
            // All ancestors
            ancestors = new List<BinaryTreeNodeAncestor<TNode>>();

            BinaryTreeNodePosition previousPosition = BinaryTreeNodePosition.Root;

            // Do binary tree search
            while (current != null)
            {
                ancestors.Add(new BinaryTreeNodeAncestor<TNode>(previousPosition, current));
                int compareResult = Compare(key, current.Key);
                if (compareResult < 0)
                {
                    rightAncestor = current;
                    current = current.LeftChild;
                    previousPosition = BinaryTreeNodePosition.LeftChild;
                }
                else if (compareResult > 0)
                {
                    leftAncestor = current;
                    current = current.RightChild;
                    previousPosition = BinaryTreeNodePosition.RightChild;
                }
                else
                {
                    break;
                }
            }

            // Get result
            if (current != null)
            {
                return current; // Exact match
            }
            else if (side == TryGetSide.Left)
            {
                return leftAncestor;
            }
            else if (side == TryGetSide.Right)
            {
                return rightAncestor;
            }
            else if (side == TryGetSide.Parent)
            {
                if (ancestors.Count > 0)
                {
                    return ancestors[ancestors.Count - 1].Node;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Clear the tree.
        /// </summary>
        public virtual void Clear()
        {
            Root = null;
            _count = 0;
        }

        /// <summary>
        /// Check whether the given key value pair is in the tree.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            TValue value;
            bool found = TryGetValue(item.Key, out value);
            if (found)
            {
                return IsValueEqual(value, item.Value);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check whether the given key is in the tree.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key)
        {
            TValue value;
            return TryGetValue(key, out value);
        }

        /// <summary>
        /// Copy all key value pairs to an array
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }
            if (arrayIndex + Count > array.Length)
            {
                throw new ArgumentException("The number of elements is greater than the available number of elements from index to the end of the destination array.", "arrayIndex");
            }
            int i = arrayIndex;
            foreach (TNode node in Nodes)
            {
                array[i] = new KeyValuePair<TKey, TValue>(node.Key, node.Value);
                i++;
            }
        }

        /// <summary>
        /// Get a enumerator for key value pairs. Make sure this tree is not changed when enumerating.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return Nodes.Select(node => new KeyValuePair<TKey, TValue>(node.Key, node.Value)).GetEnumerator();
        }

        /// <summary>
        /// Get a enumerator for key value pairs. Make sure this tree is not changed when enumerating.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Enumerate all elements in the range
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="leftInclusive"></param>
        /// <param name="rightInclusive"></param>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<TKey, TValue>> RangeQuery(TKey from, TKey to, RangeQueryFlag flag = RangeQueryFlag.IncludeLeft | RangeQueryFlag.IncludeRight, TraversalOrder order = TraversalOrder.InOrder, bool reversed = false)
        {
            return new BinarySearchTreeNodeRangeQuery<TKey, TValue, TNode>(Root, from, to, _comparer, flag, order, reversed).Select(node => new KeyValuePair<TKey, TValue>(node.Key, node.Value));
        }

        /// <summary>
        /// Remove a key value pair from this tree.
        /// </summary>
        /// <param name="item"></param>
        /// <returns>Success</returns>
        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(item.Key, TryGetSide.Exact, out ancestors);
            if (node == null)
            {
                // Key not found
                return false;
            }
            if (!IsValueEqual(item.Value, node.Value))
            {
                // Value not match
                return false;
            }
            Count--;
            RemoveFromParent(ancestors);
            return true;
        }

        /// <summary>
        /// Remove a key from this tree.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>Success</returns>
        public bool Remove(TKey key)
        {
            List<BinaryTreeNodeAncestor<TNode>> ancestors;
            TNode node = GeneralTryGetNode(key, TryGetSide.Exact, out ancestors);
            if (node == null)
            {
                // Key not found
                return false;
            }
            Count--;
            RemoveFromParent(ancestors);
            return true;
        }

        /// <summary>
        /// Remove a node from this tree.
        /// Override this method to do balancing but remember to update min and max node as well.
        /// </summary>
        /// <param name="ancestors">All ancestors of this node. Should be non-empty. The first element is the root of the tree and the last element is the node to be removed. If the node itself is not the root, the second last node is its parent.</param>
        protected virtual void RemoveFromParent(List<BinaryTreeNodeAncestor<TNode>> ancestors)
        {
            Debug.Assert(ancestors != null);
            Debug.Assert(ancestors.Count > 0);
            Debug.Assert(Root == ancestors[0].Node);
            BinaryTreeNodeAncestor<TNode> nodeRecord = ancestors[ancestors.Count - 1];
            TNode node = nodeRecord.Node;

            // Find the node that will fill the node's original position (a child of a parent or the root)
            TNode substitude = node.LeftChild;
            if (substitude == null)
            {
                // Left subtree doesn't exist
                substitude = node.RightChild;
            }
            else if (node.RightChild == null)
            {
                // Right subtree doesn't exist
                // Do nothing
            }
            else if (substitude.RightChild == null)
            {
                // Take the left leaf
                substitude.RightChild = node.RightChild;
            }
            else
            {
                // Take the largest left child
                while (substitude.RightChild.RightChild != null)
                {
                    substitude = substitude.RightChild;
                }
                TNode substitudeParent = substitude;
                substitude = substitude.RightChild;
                substitudeParent.RightChild = substitude.LeftChild;
                substitude.LeftChild = node.LeftChild;
                substitude.RightChild = node.RightChild;
            }
            
            // Update min and max and replace the removing node
            if (ancestors.Count > 1)
            {
                TNode parent = ancestors[ancestors.Count - 2].Node;
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
            else
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
        }

        /// <summary>
        /// Left rotate a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected static TNode RotateLeft(TNode node)
        {
            Debug.Assert(node.RightChild != null);
            TNode newParent = node.RightChild;
            node.RightChild = newParent.LeftChild;
            newParent.LeftChild = node;
            return newParent;
        }

        /// <summary>
        /// Right rotate a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected static TNode RotateRight(TNode node)
        {
            Debug.Assert(node.LeftChild != null);
            TNode newParent = node.LeftChild;
            node.LeftChild = newParent.RightChild;
            newParent.RightChild = node;
            return newParent;
        }

        /// <summary>
        /// Left rotate then right rotate a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected static TNode RotateRightLeft(TNode node)
        {
            Debug.Assert(node.RightChild != null);
            Debug.Assert(node.RightChild.LeftChild != null);
            TNode newParent = node.RightChild.LeftChild;
            node.RightChild.LeftChild = newParent.RightChild;
            newParent.RightChild = node.RightChild;
            node.RightChild = newParent.LeftChild;
            newParent.LeftChild = node;
            return newParent;
        }

        /// <summary>
        /// Right rotate then left rotate a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected static TNode RotateLeftRight(TNode node)
        {
            Debug.Assert(node.LeftChild != null);
            Debug.Assert(node.LeftChild.RightChild != null);
            TNode newParent = node.LeftChild.RightChild;
            node.LeftChild.RightChild = newParent.LeftChild;
            newParent.LeftChild = node.LeftChild;
            node.LeftChild = newParent.RightChild;
            newParent.RightChild = node;
            return newParent;
        }

        /// <summary>
        /// Left rotate then left rotate a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected static TNode RotateLeftLeft(TNode node)
        {
            Debug.Assert(node.RightChild != null);
            Debug.Assert(node.RightChild.RightChild != null);
            TNode newParent = node.RightChild.RightChild;
            node.RightChild.RightChild = newParent.LeftChild;
            newParent.LeftChild = node.RightChild;
            node.RightChild = newParent.LeftChild.LeftChild;
            newParent.LeftChild.LeftChild = node;
            return newParent;
        }

        /// <summary>
        /// Right rotate then right rotate a node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected static TNode RotateRightRight(TNode node)
        {
            Debug.Assert(node.LeftChild != null);
            Debug.Assert(node.LeftChild.LeftChild != null);
            TNode newParent = node.LeftChild.LeftChild;
            node.LeftChild.LeftChild = newParent.RightChild;
            newParent.RightChild = node.LeftChild;
            node.LeftChild = newParent.RightChild.RightChild;
            newParent.RightChild.RightChild = node;
            return newParent;
        }

        /// <summary>
        /// Verift the tree. i.e. the tree has current left/right node relationship.
        /// </summary>
        [Conditional("DEBUG")]
        public void Verify()
        {
            Verify(Root);
        }

        /// <summary>
        /// Verift a sub-tree. i.e. the sub-tree has current left/right node relationship.
        /// </summary>
        [Conditional("DEBUG")]
        private void Verify(TNode node)
        {
            if (node != null)
            {
                Debug.Assert(node.LeftChild == null || Compare(node.LeftChild.Key, node.Key) <= 0);
                Debug.Assert(node.RightChild == null || Compare(node.RightChild.Key, node.Key) >= 0);
                Verify(node.LeftChild);
                Verify(node.RightChild);
            }
        }

        /// <summary>
        /// Verify if a substitude node in deleting process is valid.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="substitude"></param>
        [Conditional("DEBUG")]
        protected void VerifySubstitude(TNode node, TNode substitude)
        {
            if (node.LeftChild != null)
            {
                Debug.Assert(Compare(node.LeftChild.Key, substitude.Key) < 0);
            }
            if (node.RightChild != null)
            {
                Debug.Assert(Compare(node.RightChild.Key, substitude.Key) > 0);
            }
        }
    }
}
