using MartinCl2.Collections.Generic.Nodes.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Nodes.Enumeration
{
    /// <summary>
    /// Children list for a binary tree node
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    public sealed class BinaryTreeNodeChildrenList<TKey, TValue, TNode> : IReadOnlyList<TNode>
        where TNode : AbstractBinaryTreeNode<TKey, TValue, TNode>
    {
        private readonly AbstractBinaryTreeNode<TKey, TValue, TNode> _node;

        public BinaryTreeNodeChildrenList(AbstractBinaryTreeNode<TKey, TValue, TNode> node)
        {
            _node = node;
        }

        /// <summary>
        /// Index accessor
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TNode this[int index]
        {
            get
            {
                if (index == 0)
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
                else if (index == 1)
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
        /// The number of children in this node
        /// </summary>
        public int Count
        {
            get
            {
                return (_node.LeftChild == null ? 0 : 1) + (_node.RightChild == null ? 0 : 1);
            }
        }
        
        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<TNode> GetEnumerator()
        {
            return new BinaryTreeNodeChildrenEnumerator<TKey, TValue, TNode>(_node);
        }

        /// <summary>
        /// Get an enumerator
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
