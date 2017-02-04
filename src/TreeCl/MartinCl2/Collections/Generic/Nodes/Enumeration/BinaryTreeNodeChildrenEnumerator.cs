using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MartinCl2.Collections.Generic.Nodes.Enumeration
{
    /// <summary>
    /// Enumerator of binary tree node children. Since every
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinaryTreeNodeChildrenEnumerator<TKey, TValue, TNode> : IEnumerator<TNode>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>
    {
        private int _position = -1;

        private readonly AbstractBinaryTreeNode<TKey, TValue, TNode> _node;

        public BinaryTreeNodeChildrenEnumerator(AbstractBinaryTreeNode<TKey, TValue, TNode> node)
        {
            _node = node;
        }

        /// <summary>
        /// The current node
        /// </summary>
        public TNode Current
        {
            get
            {
                if (_position == 0)
                {
                    if (_node.LeftChild == null)
                    {
                        if (_node.RightChild == null)
                        {
                            throw new IndexOutOfRangeException();
                        }
                        else
                        {
                            return _node.RightChild;
                        }
                    }
                    else
                    {
                        return _node.LeftChild;
                    }
                }
                else if (_position == 1)
                {
                    if (_node.LeftChild == null || _node.RightChild == null)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    else
                    {
                        return _node.RightChild;
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// The current child node
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
        {
        }

        /// <summary>
        /// Move to next child node
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            _position++;
            return _position >= (_node.LeftChild == null ? 0 : 1) + (_node.RightChild == null ? 0 : 1);
        }

        /// <summary>
        /// Reset the enumerator
        /// </summary>
        public void Reset()
        {
            _position = -1;
        }
    }
}
