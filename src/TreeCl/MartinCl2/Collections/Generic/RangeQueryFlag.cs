using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic
{
    /// <summary>
    /// Option flags for range queries
    /// </summary>
    public enum RangeQueryFlag
    {
        /// <summary>
        /// No flag
        /// </summary>
        None = 0,
        /// <summary>
        /// Include left bound
        /// </summary>
        IncludeLeft = 1,
        /// <summary>
        /// Include right bound
        /// </summary>
        IncludeRight = 2,
        /// <summary>
        /// Treat left bound as infinity
        /// </summary>
        NoLeftBound = 4,
        /// <summary>
        /// Treat right bound as infinity
        /// </summary>
        NoRightBound = 8
    }
}
