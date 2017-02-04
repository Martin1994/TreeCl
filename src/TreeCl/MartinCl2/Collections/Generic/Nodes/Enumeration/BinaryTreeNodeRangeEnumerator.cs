using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes.Enumeration
{
    /// <summary>
    /// Enumerator for a range query on a binary search tree
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinaryTreeNodeRangeEnumerator<TKey, TValue, TNode>
        : IEnumerator<TNode> where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>
    {
        /// <summary>
        /// The state of traversal
        /// </summary>
        private enum TraversalState
        {
            /// <summary>
            /// Default state. Unused.
            /// </summary>
            None = 0,
            /// <summary>
            /// Need to report itself.
            /// </summary>
            Self = 1,
            /// <summary>
            /// Need to report its left sub-tree.
            /// </summary>
            Left = 2,
            /// <summary>
            /// Need to report its right sub-tree.
            /// </summary>
            Right = 3,
            /// <summary>
            /// Done traversal for this node.
            /// </summary>
            Done = 4
        }

        /// <summary>
        /// Node position type for range query
        /// </summary>
        private enum NodeType
        {
            // TODO: use bit flag to determine whether this type needs to be reported.
            /// <summary>
            /// The search root between left and right bound
            /// </summary>
            SearchRootInside = 0,
            /// <summary>
            /// The search root equals to both left and right bound
            /// </summary>
            SearchRootExact,
            /// <summary>
            /// The search root equals to left bound
            /// </summary>
            SearchRootLeftExact,
            /// <summary>
            /// The search root equals to right bound
            /// </summary>
            SearchRootRightExact,
            /// <summary>
            /// Node between left and right edges
            /// </summary>
            Inside,
            /// <summary>
            /// Node on the left edge and right to the left bound
            /// </summary>
            LeftEdgeInside,
            /// <summary>
            /// Node on the right edge and left to the right bound
            /// </summary>
            RightEdgeInside,
            /// <summary>
            /// Node on the left edge and left to the left bound
            /// </summary>
            LeftEdgeOutside,
            /// <summary>
            /// Node on the right edge and right to the right bound
            /// </summary>
            RightEdgeOutside,
            /// <summary>
            /// Node on the left edge and equals to the left bound
            /// </summary>
            LeftExact,
            /// <summary>
            /// Node on the right edge and equals to the right bound
            /// </summary>
            RightExact
        }

        /// <summary>
        /// Whether a node type needs to report itself (i.e. in the range)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool NeedReport(NodeType type)
        {
            return type != NodeType.LeftEdgeOutside && type != NodeType.RightEdgeOutside && (type != NodeType.LeftExact || _leftInclusive) && (type != NodeType.RightExact || _rightInclusive);
        }
        
        /// <summary>
        /// Root of the tree
        /// </summary>
        private readonly TNode _root;
        /// <summary>
        /// Left bound
        /// </summary>
        private readonly TKey _from;
        /// <summary>
        /// Right bound
        /// </summary>
        private readonly TKey _to;
        /// <summary>
        /// Whether left bound is in the range
        /// </summary>
        private readonly bool _leftInclusive;
        /// <summary>
        /// Whether right bound is in the range
        /// </summary>
        private readonly bool _rightInclusive;
        /// <summary>
        /// Whether there is no left bound
        /// </summary>
        private readonly bool _leftAll;
        /// <summary>
        /// Whether there is not right bound
        /// </summary>
        private readonly bool _rightAll;
        /// <summary>
        /// Comparer of keys
        /// </summary>
        private readonly IComparer<TKey> _comparer;

        /// <summary>
        /// Traversal path of nodes
        /// </summary>
        private readonly List<TNode> _path = new List<TNode>();
        /// <summary>
        /// Done traversal state along the traversal path
        /// </summary>
        private readonly List<int> _pathTraverseState = new List<int>();
        /// <summary>
        /// Traversal node position type along the traversal path
        /// </summary>
        private readonly List<NodeType> _pathSearchState = new List<NodeType>();

        /// <summary>
        /// Order of traversal state in a single node.
        /// Will be generated in constructor depending on order and reversed parameter
        /// </summary>
        private readonly TraversalState[] _traversalStateOrder = new TraversalState[4];

        /// <summary>
        /// Whether this enumerator has started
        /// </summary>
        private bool _started;

        /// <summary>
        /// Compare two keys
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private int Compare(TKey lhs, TKey rhs)
        {
            return _comparer.Compare(lhs, rhs);
        }

        public BinaryTreeNodeRangeEnumerator(TNode root, TKey from, TKey to, IComparer<TKey> comparer, RangeQueryFlag flag = RangeQueryFlag.IncludeLeft | RangeQueryFlag.IncludeRight, TraversalOrder order = TraversalOrder.InOrder, bool reversed = false)
        {
            _root = root;
            _from = from;
            _to = to;
            _leftInclusive = flag.HasFlag(RangeQueryFlag.IncludeLeft);
            _rightInclusive = flag.HasFlag(RangeQueryFlag.IncludeRight);
            _leftAll = flag.HasFlag(RangeQueryFlag.NoLeftBound);
            _rightAll = flag.HasFlag(RangeQueryFlag.NoRightBound);
            _comparer = comparer;
            Debug.Assert(_leftAll && _rightAll || _comparer != null);

            // Check if enumeration is actually needed
            _started = ShouldStart();

            if (order == TraversalOrder.InOrder)
            {
                if (!reversed)
                {
                    _traversalStateOrder[0] = TraversalState.Left;
                    _traversalStateOrder[1] = TraversalState.Self;
                    _traversalStateOrder[2] = TraversalState.Right;
                }
                else
                {
                    _traversalStateOrder[0] = TraversalState.Right;
                    _traversalStateOrder[1] = TraversalState.Self;
                    _traversalStateOrder[2] = TraversalState.Left;
                }
            }
            else if (order == TraversalOrder.PreOrder)
            {
                if (!reversed)
                {
                    _traversalStateOrder[0] = TraversalState.Self;
                    _traversalStateOrder[1] = TraversalState.Left;
                    _traversalStateOrder[2] = TraversalState.Right;
                }
                else
                {
                    _traversalStateOrder[0] = TraversalState.Self;
                    _traversalStateOrder[1] = TraversalState.Right;
                    _traversalStateOrder[2] = TraversalState.Left;
                }
            }
            else // PostOrder
            {
                if (!reversed)
                {
                    _traversalStateOrder[0] = TraversalState.Left;
                    _traversalStateOrder[1] = TraversalState.Right;
                    _traversalStateOrder[2] = TraversalState.Self;
                }
                else
                {
                    _traversalStateOrder[0] = TraversalState.Right;
                    _traversalStateOrder[1] = TraversalState.Left;
                    _traversalStateOrder[2] = TraversalState.Self;
                }
            }
            _traversalStateOrder[3] = TraversalState.Done;
        }

        /// <summary>
        /// Current node in the range
        /// </summary>
        public TNode Current
        {
            get
            {
                if (_path.Count == 0)
                {
                    throw new InvalidOperationException();
                }
                return _path.Last();
            }
        }

        /// <summary>
        /// Next node in the range
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// Dispose this enumerator
        /// </summary>
        public void Dispose()
        { }

        /// <summary>
        /// Whether this enumerator should start (i.e. may contains at least one node)
        /// </summary>
        /// <returns></returns>
        private bool ShouldStart()
        {
            int compareResult = Compare(_from, _to);
            return _root != null && (_leftAll || _rightAll || compareResult < 0 || compareResult == 0 && _leftInclusive && _rightInclusive);
        }

        /// <summary>
        /// Move to next node in the range
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (!_started)
            {
                return false;
            }

            if (_path.Count == 0)
            {
                // Start to enumerate
                // Try to find the searching root
                TNode start = _root;
                int leftCompare = -1;
                int rightCompare = 1;
                while (start != null && (leftCompare < 0 || rightCompare > 0))
                {
                    leftCompare = _leftAll ? 1 : Compare(start.Key, _from);
                    rightCompare = _rightAll ? -1 : Compare(start.Key, _to);
                    Debug.Assert(leftCompare >= rightCompare);
                    if (!_leftInclusive && leftCompare == 0)
                    {
                        leftCompare = -1;
                    }
                    if (!_rightInclusive && rightCompare == 0)
                    {
                        rightCompare = 1;
                    }

                    if (leftCompare < 0)
                    {
                        start = start.RightChild;
                    }
                    else if (rightCompare > 0)
                    {
                        start = start.LeftChild;
                    }
                }
                if (start != null)
                {
                    // Found starting node
                    _path.Add(start);
                    _pathTraverseState.Add(-1);
                    if (leftCompare == 0 && rightCompare == 0)
                    {
                        _pathSearchState.Add(NodeType.SearchRootExact);
                    }
                    else if (leftCompare == 0)
                    {
                        _pathSearchState.Add(NodeType.SearchRootLeftExact);
                    }
                    else if (rightCompare == 0)
                    {
                        _pathSearchState.Add(NodeType.SearchRootRightExact);
                    }
                    else
                    {
                        _pathSearchState.Add(NodeType.SearchRootInside);
                    }
                }
                else
                {
                    // No such nodes
                    _started = false;
                    return false;
                }
            }

            // Continue traversing from the last call
            int pathIndex = _path.Count - 1;
            while (true)
            {
                TNode child;
                int compareResult;
                switch (_traversalStateOrder[++_pathTraverseState[pathIndex]])
                {
                    case TraversalState.Self:
                        if (NeedReport(_pathSearchState[pathIndex]))
                        {
                            return true;
                        }
                        else
                        {
                            break;
                        }

                    case TraversalState.Left:
                        child = _path[pathIndex].LeftChild;
                        if (child != null)
                        {
                            switch (_pathSearchState[pathIndex])
                            {
                                case NodeType.Inside:
                                case NodeType.RightEdgeInside:
                                case NodeType.RightExact:
                                    _path.Add(child);
                                    _pathTraverseState.Add(-1);
                                    _pathSearchState.Add(NodeType.Inside);
                                    pathIndex++;
                                    break;

                                case NodeType.SearchRootInside:
                                case NodeType.SearchRootRightExact:
                                    if (_leftAll)
                                    {
                                        _path.Add(child);
                                        _pathTraverseState.Add(-1);
                                        _pathSearchState.Add(NodeType.Inside);
                                        pathIndex++;
                                        break;
                                    } 
                                    else
                                    {
                                        goto case NodeType.LeftEdgeInside;
                                    }

                                case NodeType.LeftEdgeInside:
                                    compareResult = Compare(child.Key, _from);
                                    _path.Add(child);
                                    _pathTraverseState.Add(-1);
                                    if (compareResult < 0)
                                    {
                                        _pathSearchState.Add(NodeType.LeftEdgeOutside);
                                    }
                                    else if (compareResult == 0)
                                    {
                                        _pathSearchState.Add(NodeType.LeftExact);
                                    }
                                    else // compareResult > 0
                                    {
                                        _pathSearchState.Add(NodeType.LeftEdgeInside);
                                    }
                                    pathIndex++;
                                    break;

                                case NodeType.RightEdgeOutside:
                                    compareResult = Compare(child.Key, _to);
                                    _path.Add(child);
                                    _pathTraverseState.Add(-1);
                                    if (compareResult < 0)
                                    {
                                        _pathSearchState.Add(NodeType.RightEdgeInside);
                                    }
                                    else if (compareResult == 0)
                                    {
                                        _pathSearchState.Add(NodeType.RightExact);
                                    }
                                    else // compareResult > 0
                                    {
                                        _pathSearchState.Add(NodeType.RightEdgeOutside);
                                    }
                                    pathIndex++;
                                    break;
                            }
                        }
                        break;

                    case TraversalState.Right:
                        child = _path[pathIndex].RightChild;
                        if (child != null)
                        {
                            switch (_pathSearchState[pathIndex])
                            {
                                case NodeType.Inside:
                                case NodeType.LeftEdgeInside:
                                case NodeType.LeftExact:
                                    _path.Add(child);
                                    _pathTraverseState.Add(-1);
                                    _pathSearchState.Add(NodeType.Inside);
                                    pathIndex++;
                                    break;

                                case NodeType.SearchRootInside:
                                case NodeType.SearchRootLeftExact:
                                    if (_rightAll)
                                    {
                                        _path.Add(child);
                                        _pathTraverseState.Add(-1);
                                        _pathSearchState.Add(NodeType.Inside);
                                        pathIndex++;
                                        break;
                                    }
                                    else
                                    {
                                        goto case NodeType.RightEdgeInside;
                                    }

                                case NodeType.RightEdgeInside:
                                    compareResult = Compare(child.Key, _to);
                                    _path.Add(child);
                                    _pathTraverseState.Add(-1);
                                    if (compareResult < 0)
                                    {
                                        _pathSearchState.Add(NodeType.RightEdgeInside);
                                    }
                                    else if (compareResult == 0)
                                    {
                                        _pathSearchState.Add(NodeType.RightExact);
                                    }
                                    else // compareResult > 0
                                    {
                                        _pathSearchState.Add(NodeType.RightEdgeOutside);
                                    }
                                    pathIndex++;
                                    break;

                                case NodeType.LeftEdgeOutside:
                                    compareResult = Compare(child.Key, _from);
                                    _path.Add(child);
                                    _pathTraverseState.Add(-1);
                                    if (compareResult < 0)
                                    {
                                        _pathSearchState.Add(NodeType.LeftEdgeOutside);
                                    }
                                    else if (compareResult == 0)
                                    {
                                        _pathSearchState.Add(NodeType.LeftExact);
                                    }
                                    else // compareResult > 0
                                    {
                                        _pathSearchState.Add(NodeType.LeftEdgeInside);
                                    }
                                    pathIndex++;
                                    break;
                            }
                        }
                        break;

                    case TraversalState.Done:
                        _path.RemoveAt(pathIndex);
                        _pathTraverseState.RemoveAt(pathIndex);
                        _pathSearchState.RemoveAt(pathIndex);
                        if (_path.Count == 0)
                        {
                            _started = false;
                            return false;
                        }
                        pathIndex--;
                        break;
                }
            }
        }

        /// <summary>
        /// Reset the enumerator
        /// </summary>
        public void Reset()
        {
            _path.Clear();
            _pathTraverseState.Clear();
            _pathSearchState.Clear();
            _started = ShouldStart();
        }
    }
}
