using Xunit;
using Elevator;
using System;

namespace ElevatorSimulatorTest
{
    public class SortedLinkedListTest
    {
        private Comparison<int> compareLogic = (a, b) => a.CompareTo(b);

        [Fact]
        public void EmptyList()
        {
            var list = new SortedLinkedList<Int32>(compareLogic);

            Assert.NotNull(list);
            Assert.Equal(0, list.Count);
            Assert.Equal(default(Int32), list.First);
        }

        [Fact]
        public void AddElementToEmptyList()
        {
            var list = new SortedLinkedList<int>(compareLogic);
            list.Add(1);

            Assert.Equal(list.Count, 1);
            Assert.Equal(list.First, 1);
        }
    }
}
