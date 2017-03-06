using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MartinCl2.Collections.Generic.Tests
{
    /// <summary>
    /// General test class for a variant of binary search tree.
    /// Note that although all the test cases work with non-binary search tree (e.g. a-b tree), they are designed for a binary search tree (has an expected balanced shape).
    /// </summary>
    public abstract class IBinarySearchTreeTest
    {
        protected abstract IBinarySearchTree<TKey, TValue> CreateBST<TKey, TValue>();
        protected abstract IBinarySearchTree<TKey, TValue> CreateBST<TKey, TValue>(ICollection<KeyValuePair<TKey, TValue>> items);

        [TestMethod]
        public void EmptyBST()
        {
            IBinarySearchTree<int, int> bst = CreateBST<int, int>();
            foreach(KeyValuePair<int, int> kvp in bst)
            {
                Assert.Fail();
            }
            Assert.AreEqual(bst.Count, 0);
        }

        [TestMethod]
        public void OneNodeBST()
        {
            IBinarySearchTree<int, string> bst = CreateBST<int, string>();
            bst.Add(42, "Universe");
            int loopCount = 0;
            foreach (KeyValuePair<int, string> kvp in bst)
            {
                Assert.AreEqual(kvp.Key, 42);
                Assert.AreEqual(kvp.Value, "Universe");
                loopCount++;
            }
            Assert.AreEqual(bst.Count, 1);
            Assert.AreEqual(bst.Count, loopCount);
        }

        [TestMethod]
        public void SmallTest()
        {
            List<List<int>> tests = new List<List<int>>()
            {
                new List<int>() { 1 },
                new List<int>() { 1, 2 },
                new List<int>() { 1, 2, 3 },
                new List<int>() { 2, 1, 3 },
                new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                new List<int>() { 3, 14, 1, -5, 9, 26, 5 }
            };
            foreach (List<int> numbers in tests)
            {
                OrderedListBST(numbers);
                SortedListBST(numbers);
            }
        }

        [TestMethod]
        public void BigTest()
        {
            List<int> numbers = new List<int>()
            {
                100, 56, 11, 21, 68, 86, 31, 14, 49, 64, 84, 73, 72, 52, 48, 81, 23, 83, 54, 3, 98, 90, 6, 77, 41, 51, 96, 20, 75, 33, 60, 10, 87, 65, 1, 67, 89, 28, 26, 30, 5, 42, 66, 32, 47, 58, 76, 53, 7, 29, 78, 40, 44, 15, 39, 46, 82, 71, 36, 57, 34, 99, 63, 17, 55, 69, 91, 38, 16, 4, 19, 50, 62, 2, 45, 61, 13, 93, 94, 88, 74, 8, 80, 18, 25, 12, 79, 85, 22, 24, 37, 97, 27, 9, 43, 35, 95, 70, 59, 92
            };
            OrderedListBST(numbers);
            SortedListBST(numbers);
        }

        [TestMethod]
        public void RandomTest()
        {
            IBinarySearchTree<byte, int> bst = CreateBST<byte, int>();
            for (int i = 0; i < 100000; i++)
            {
                Random rnd = new Random();
                int value = rnd.Next();
                byte key = (byte)(value % (int)byte.MaxValue);
                bst[key] = value;
                Assert.AreEqual(value, bst[key]);
                if (i % 10 == 0)
                {
                    bst.Remove(bst.Max.Key);
                }
                if (i % 10 == 5)
                {
                    bst.Remove(bst.Min.Key);
                }
            }
        }

        [TestMethod]
        public void OverrideTest()
        {
            IBinarySearchTree<int, int> bst = CreateBST<int, int>();
            bst[0] = 1;
            bst[0] = -1;
            bst[0] = 0;
            bst[1] = -1;
            bst[-1] = 1;
            Assert.AreEqual(bst[0], 0);
            Assert.AreEqual(bst[1], -1);
            Assert.AreEqual(bst[-1], 1);
            Assert.AreEqual(bst.Count, 3);
            bst[0] = 0;
            bst[1] = 1;
            bst[-1] = -1;
            bst[0] = 0;
            Assert.AreEqual(bst[0], 0);
            Assert.AreEqual(bst[1], 1);
            Assert.AreEqual(bst[-1], -1);
            Assert.AreEqual(bst.Count, 3);
        }

        [TestMethod]
        public void LeftRightTest()
        {
            IBinarySearchTree<int, int> bst = CreateBST<int, int>(new List<int>()
            {
                1, 3, 5, 7, 9, 11, 13
            }.Select(num => new KeyValuePair<int, int>(num, num)).ToList());

            int value;

            Assert.IsTrue(bst.TryGetValue(5, out value));
            Assert.AreEqual(5, value);

            Assert.IsFalse(bst.TryGetValue(6, out value));

            Assert.IsTrue(bst.TryGetLeftClosestValue(6, out value));
            Assert.AreEqual(5, value);

            Assert.IsTrue(bst.TryGetRightClosestValue(6, out value));
            Assert.AreEqual(7, value);

            Assert.IsTrue(bst.TryGetLeftClosestValue(11, out value));
            Assert.AreEqual(11, value);

            Assert.IsTrue(bst.TryGetRightClosestValue(11, out value));
            Assert.AreEqual(11, value);

            Assert.IsTrue(bst.TryGetLeftClosestValue(14, out value));
            Assert.AreEqual(13, value);

            Assert.IsTrue(bst.TryGetRightClosestValue(0, out value));
            Assert.AreEqual(1, value);

            Assert.IsFalse(bst.TryGetLeftClosestValue(0, out value));

            Assert.IsFalse(bst.TryGetRightClosestValue(14, out value));
        }

        private void SortedListBST(List<int> numbers)
        {
            numbers = numbers.ToList();
            IBinarySearchTree<int, int> bst = CreateBST<int, int>(
                numbers.Select(number => new KeyValuePair<int, int>(number, number)).ToList()
            );
            numbers.Sort();
            List<int> sorted = bst.Keys.ToList();
            Assert.AreEqual(sorted.Count, numbers.Count);
            for (int i = 0; i < numbers.Count; i++)
            {
                Assert.AreEqual(numbers[i], sorted[i]);
                Assert.AreEqual(numbers[i], bst[numbers[i]]);
            }
        }

        private void OrderedListBST(List<int> numbers)
        {
            IBinarySearchTree<int, int> bst = CreateBST<int, int>();
            foreach (int number in numbers)
            {
                bst.Add(number, number);
            }
            List<int> sorted = bst.Keys.ToList();
            Assert.AreEqual(sorted.Count, numbers.Count);
            int last = sorted[0];
            foreach (int number in sorted)
            {
                Assert.IsTrue(last <= number);
                last = number;
                Assert.IsTrue(numbers.Contains(number));
            }
            foreach (int number in numbers)
            {
                Assert.AreEqual(number, bst[number]);
            }
        }

        [TestMethod]
        public void DeleteTest()
        {
            List<int> nums = new List<int>()
            {
                1, 2, 3, 4, 5, 6, 7
            };
            foreach (int toDelete in nums)
            {
                IBinarySearchTree<int, int> bst = CreateBST<int, int>(nums.Select(num => new KeyValuePair<int, int>(num, num)).ToList());
                bst.Remove(toDelete);
                List<int> postDeletedNums = nums.Where(num => num != toDelete).ToList();
                List<int> postDeletedBst = bst.Select(kvp => kvp.Key).ToList();
                Assert.AreEqual(postDeletedNums.Count, bst.Count);
                AssertListAreEqual(postDeletedNums, postDeletedBst);
            }
        }

        private void AssertListAreEqual<T>(List<T> expected, List<T> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod]
        public void RangeQueryTest()
        {
            List<int> nums = new List<int>()
            {
                1, 2, 3, 4, 5, 6, 7
            };
            IBinarySearchTree<int, int> bst = CreateBST(nums.Select(num => new KeyValuePair<int, int>(num, num)).ToList());

            List<int> query = bst.RangeQuery(0, 8, RangeQueryFlag.None, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(nums, query);

            query = bst.RangeQuery(1, 7, RangeQueryFlag.IncludeLeft | RangeQueryFlag.IncludeRight, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(nums, query);

            query = bst.RangeQuery(/*DONT CARE*/999, /*DONT CARE*/0, RangeQueryFlag.NoLeftBound | RangeQueryFlag.NoRightBound, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(nums, query);

            query = bst.RangeQuery(1, 7, RangeQueryFlag.None, TraversalOrder.InOrder, true).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 6, 5, 4, 3, 2 }, query);

            query = bst.RangeQuery(4, /*DONT CARE*/0, RangeQueryFlag.IncludeLeft | RangeQueryFlag.NoRightBound, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 4, 5, 6, 7 }, query);

            query = bst.RangeQuery(3, /*DONT CARE*/0, RangeQueryFlag.IncludeLeft | RangeQueryFlag.NoRightBound, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 3, 4, 5, 6, 7 }, query);

            query = bst.RangeQuery(/*DONT CARE*/999, 4, RangeQueryFlag.NoLeftBound, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 1, 2, 3 }, query);

            query = bst.RangeQuery(4, 4, RangeQueryFlag.IncludeLeft | RangeQueryFlag.IncludeRight, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 4 }, query);

            query = bst.RangeQuery(4, 4, RangeQueryFlag.None, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>(), query);

            query = bst.RangeQuery(4, 4, RangeQueryFlag.IncludeLeft, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>(), query);

            query = bst.RangeQuery(4, 4, RangeQueryFlag.IncludeRight, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>(), query);

            query = bst.RangeQuery(3, 3, RangeQueryFlag.IncludeLeft | RangeQueryFlag.IncludeRight, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 3 }, query);

            query = bst.RangeQuery(9, 10, RangeQueryFlag.None, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>(), query);

            query = bst.RangeQuery(7, 1, RangeQueryFlag.None, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>(), query);

            query = bst.RangeQuery(5, 7, RangeQueryFlag.IncludeLeft, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 5, 6 }, query);

            query = bst.RangeQuery(5, 7, RangeQueryFlag.IncludeRight, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>() { 6, 7 }, query);

            List<int> nums2 = new List<int>()
            {
                -9, -6, -3, 0, 3, 6, 9
            };
            IBinarySearchTree<int, int> bst2 = CreateBST(nums2.Select(num => new KeyValuePair<int, int>(num, num)).ToList());

            query = bst2.RangeQuery(-5, -4, RangeQueryFlag.IncludeLeft, TraversalOrder.InOrder, false).Select(kvp => kvp.Key).ToList();
            AssertListAreEqual(new List<int>(), query);
        }
    }
}
