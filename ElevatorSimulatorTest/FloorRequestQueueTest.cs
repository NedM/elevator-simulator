using Elevator;
using System;
using Xunit;

namespace ElevatorSimulatorTest
{
    public class FloorRequestQueueTest
    {
        private Comparison<FloorRequest> compareLogic_Ascending = (a, b) => a.CompareTo(b);

        [Fact]
        public void AddMultipleElementsToQueue_SameDirection()
        {
            FloorRequest request = new FloorRequest(5, Direction.Up);
            FloorRequestQueue queue = new FloorRequestQueue(compareLogic_Ascending);

            queue.Add(request);

            Assert.Equal(1, CountElements(queue));

            queue.Add(request);
            queue.Add(request);

            Assert.Equal(1, CountElements(queue));
        }

        [Fact]
        public void AddMultipleElementsToQueue_DifferentDirection()
        {
            Floor floor = new Floor(5, "Test Floor");
            FloorRequest request1 = new FloorRequest(floor, Direction.Up);
            FloorRequest request2 = new FloorRequest(floor, Direction.Down);
            FloorRequest request3 = new FloorRequest(floor, Direction.None);

            FloorRequestQueue queue = new FloorRequestQueue(compareLogic_Ascending);

            queue.Add(request1);
            Assert.Equal(1, CountElements(queue));

            queue.Add(request2);
            Assert.Equal(2, CountElements(queue));

            queue.Add(request3);
            Assert.Equal(2, CountElements(queue));
        }

        private int CountElements(FloorRequestQueue queue)
        {
            int count = 0;
            foreach(FloorRequest fr in queue)
            {
                count++;
            }

            return count;
        }

    }
}
