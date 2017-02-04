using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic
{
    /// <summary>
    /// Tree traversal order
    /// <see cref="http://en.wikipedia.org/wiki/Tree_traversal"/>
    /// </summary>
    public enum TraversalOrder
    {
        /// <summary>
        /// Current node -> left recursion -> right recursion
        /// </summary>
        PreOrder = -1,
        /// <summary>
        /// left recursion -> Current node -> right recursion
        /// </summary>
        InOrder = 0,
        /// <summary>
        /// left recursion -> right recursion -> Current node
        /// </summary>
        PostOrder = 1
    }
}
