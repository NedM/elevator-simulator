using System;
using System.Collections;

namespace Elevator
{
    public class FloorRequestQueue
    {
        private SortedLinkedList<FloorRequest> _queue;

        private readonly Comparison<FloorRequest> _comparisonLogic;

        public FloorRequestQueue(Comparison<FloorRequest> comparisonLogic = null)
        {
            _queue = new SortedLinkedList<FloorRequest>(comparisonLogic);
            _comparisonLogic = comparisonLogic;            
        }

        public bool Any { get { return _queue.First != null; } }

        public void Add(FloorRequest request)
        {
            _queue.Add(request);
        }        

        public FloorRequest Peek()
        {
            return _queue.First?.Value;
        }

        public FloorRequest Dequeue()
        {
            FloorRequest request = Peek();
            _queue.Remove(request);
            return request;
        }

        public void Remove(FloorRequest request)
        {
            _queue.Remove(request);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _queue)}]";
        }
    }
}