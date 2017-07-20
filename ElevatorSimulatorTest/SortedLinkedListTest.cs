using Xunit;
using Elevator;
using System;

namespace ElevatorSimulatorTest
{
    public class SortedLinkedListTest
    {
        private Comparison<int> compareLogic_Ascending = (a, b) => a.CompareTo(b);
        private Comparison<int> compareLogic_Descending = (a, b) => b.CompareTo(a);

        [Fact]
        public void EmptyList()
        {
            var list = new SortedLinkedList<Int32>(compareLogic_Ascending);

            Assert.NotNull(list);
            Assert.Equal(0, list.Count);
            Assert.Equal(null, list.First?.Value);
        }

        [Fact]
        public void AddSingleElementToEmptyList()
        {
            int element = 34;
            var list = new SortedLinkedList<int>(compareLogic_Ascending);
            list.Add(element);

            Assert.Equal(list.Count, 1);
            Assert.Equal(list.First.Value, element);
        }

        [Fact]
        public void AddMultipleElementsToList_OrderedAscending_SortedAscending()
        {
            int[] ordered = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var list = new SortedLinkedList<int>(compareLogic_Ascending);

            foreach(int i in ordered)
            {
                list.Add(i);
            }

            Assert.Equal(list.Count, ordered.Length);
            Assert.Equal(list.ToArray(), ordered);
        }

        [Fact]
        public void AddMultipleElementsToList_OrderedDescending_SortedAscending()
        {
            int[] ordered = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1};
            var list = new SortedLinkedList<int>(compareLogic_Ascending);

            foreach (int i in ordered)
            {
                list.Add(i);
            }

            Array.Reverse(ordered);

            Assert.Equal(list.Count, ordered.Length);
            Assert.Equal(list.ToArray(), ordered);
        }

        [Fact]
        public void AddMultipleElementsToList_Unordered_SortedAscending()
        {
            int[] unordered = new int[] { 5, 9, 1, 6, 4, 7, 2, 3, 8, 10 };
            var list = new SortedLinkedList<int>(compareLogic_Ascending);

            foreach (int i in unordered)
            {
                list.Add(i);
            }

            Array.Sort(unordered);

            Assert.Equal(list.Count, unordered.Length);
            Assert.Equal(list.ToArray(), unordered);
        }

        [Fact]
        public void AddMultipleElementsToList_OrderedAscending_SortedDescending()
        {
            int[] ordered = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var list = new SortedLinkedList<int>(compareLogic_Descending);

            foreach (int i in ordered)
            {
                list.Add(i);
            }

            Array.Reverse(ordered);

            Assert.Equal(list.Count, ordered.Length);
            Assert.Equal(list.ToArray(), ordered);
        }

        [Fact]
        public void AddMultipleElementsToList_OrderedDescending_SortedDescending()
        {
            int[] ordered = new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            var list = new SortedLinkedList<int>(compareLogic_Descending);

            foreach (int i in ordered)
            {
                list.Add(i);
            }
            
            Assert.Equal(list.Count, ordered.Length);
            Assert.Equal(list.ToArray(), ordered);
        }

        [Fact]
        public void AddMultipleElementsToList_Unordered_SortedDescending()
        {
            int[] unordered = new int[] { 5, 9, 1, 6, 4, 10, 7, 2, 3, 8 };
            var list = new SortedLinkedList<int>(compareLogic_Descending);

            foreach (int i in unordered)
            {
                list.Add(i);
            }

            Array.Sort(unordered, (a, b) => b.CompareTo(a));

            Assert.Equal(list.Count, unordered.Length);
            Assert.Equal(list.ToArray(), unordered);
        }
    }
}
