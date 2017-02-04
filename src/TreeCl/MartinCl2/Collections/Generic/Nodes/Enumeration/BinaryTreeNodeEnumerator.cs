using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes.Enumeration
{
    /// <summary>
    /// Enumerator for all nodes in a binary tree
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinaryTreeNodeEnumerator<TKey, TValue, TNode>
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
        /// Root of the tree
        /// </summary>
        private readonly TNode _root;

        /// <summary>
        /// Traversal path of nodes
        /// </summary>
        private readonly List<TNode> _path = new List<TNode>();
        /// <summary>
        /// Done traversal state along the traversal path
        /// </summary>
        private readonly List<int> _pathDoneState = new List<int>();

        /// <summary>
        /// Order of traversal state in a single node.
        /// Will be generated in constructor depending on order and reversed parameter
        /// </summary>
        private readonly TraversalState[] _traversalStateOrder = new TraversalState[4];

        /// <summary>
        /// Whether this enumerator has started
        /// </summary>
        private bool _started = true;

        public BinaryTreeNodeEnumerator(TNode root, TraversalOrder order = TraversalOrder.InOrder, bool reversed = false)
        {
            _root = root;
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
        /// The current node
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
        /// The current node
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
        /// Move to next node
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            if (!_started)
            {
                return false;
            }

            if (_root == null)
            {
                _started = false;
                return false;
            }

            if (_path.Count == 0)
            {
                _path.Add(_root);
                _pathDoneState.Add(-1);
            }

            // Continue traversing from the last call
            int pathIndex = _path.Count - 1;
            while (true)
            {
                TNode child;
                switch (_traversalStateOrder[++_pathDoneState[pathIndex]])
                {
                    case TraversalState.Self:
                        return true;

                    case TraversalState.Left:
                        child = _path[pathIndex].LeftChild;
                        if (child != null)
                        {
                            _path.Add(child);
                            _pathDoneState.Add(-1);
                            pathIndex++;
                        }
                        break;

                    case TraversalState.Right:
                        child = _path[pathIndex].RightChild;
                        if (child != null)
                        {
                            _path.Add(child);
                            _pathDoneState.Add(-1);
                            pathIndex++;
                        }
                        break;

                    case TraversalState.Done:
                        _path.RemoveAt(pathIndex);
                        _pathDoneState.RemoveAt(pathIndex);
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
            _pathDoneState.Clear();
            _started = true;
        }
    }
}
