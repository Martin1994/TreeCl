using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic
{
    /// <summary>
    /// Binary search tree interface.
    /// This interface defines some common operations amoung every variant of binary search trees.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IBinarySearchTree<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// Get all keys in the tree
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        ICollection<TKey> GetKeys(TraversalOrder order = TraversalOrder.InOrder, bool reversed = false);

        /// <summary>
        /// Get all values in the tree
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        ICollection<TValue> GetValues(TraversalOrder order = TraversalOrder.InOrder, bool reversed = false);

        /// <summary>
        /// Get all key value pairs in the tree
        /// </summary>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        ICollection<KeyValuePair<TKey, TValue>> GetKeyValuePairs(TraversalOrder order = TraversalOrder.InOrder, bool reversed = false);

        /// <summary>
        /// Get the most right possible value whose key is left to the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetLeftClosestValue(TKey key);

        /// <summary>
        /// Try to get the most right possible value whose key is left to the given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resultValue"></param>
        /// <returns>success</returns>
        bool TryGetLeftClosestValue(TKey key, out TValue resultValue);

        /// <summary>
        /// Try to get the most right possible value whose key is left to the given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resultKey"></param>
        /// <param name="resultValue"></param>
        /// <returns>success</returns>
        bool TryGetLeftClosestValue(TKey key, out TKey resultKey, out TValue resultValue);

        /// <summary>
        /// Get the most left possible value whose key is right to the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        TValue GetRightClosestValue(TKey key);

        /// <summary>
        /// Try to get the most left possible value whose key is right to the given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        bool TryGetRightClosestValue(TKey key, out TValue resultValue);

        /// <summary>
        /// Try to get the most left possible value whose key is right to the given key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="resultKey"></param>
        /// <param name="resultValue"></param>
        /// <returns></returns>
        bool TryGetRightClosestValue(TKey key, out TKey resultKey, out TValue resultValue);

        /// <summary>
        /// Do a range query on this tree
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="flag"></param>
        /// <param name="order"></param>
        /// <param name="reversed"></param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<TKey, TValue>> RangeQuery(TKey from, TKey to, RangeQueryFlag flag = RangeQueryFlag.IncludeLeft | RangeQueryFlag.IncludeRight, TraversalOrder order = TraversalOrder.InOrder, bool reversed = false);
    }
}
