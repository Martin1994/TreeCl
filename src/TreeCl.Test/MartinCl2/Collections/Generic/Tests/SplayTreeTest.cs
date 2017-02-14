using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Tests
{
    [TestClass]
    public class SplayTreeTest : IBinarySearchTreeTest
    {
        protected override IBinarySearchTree<TKey, TValue> CreateBST<TKey, TValue>()
        {
            return new SplayTree<TKey, TValue>();
        }

        protected override IBinarySearchTree<TKey, TValue> CreateBST<TKey, TValue>(ICollection<KeyValuePair<TKey, TValue>> items)
        {
            return new SplayTree<TKey, TValue>(items);
        }
    }
}
