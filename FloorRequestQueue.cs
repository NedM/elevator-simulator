using log4net;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Elevator
{
    public class FloorRequestQueue : IEnumerable, IEnumerable<FloorRequest>
    {
        private ILog _log = LogManager.GetLogger(typeof(ElevatorSystem));

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
            if (_queue.Contains(request))
            {
                _log.Debug($"Queue already contains a {request}. Discarding....");
                return;
            }

            _queue.Add(request);
            _log.Debug($"Added request {request} to queue. The queue currently holds {_queue.Count} requests.");
        }

        public FloorRequest Peek()
        {
            return _queue.First?.Value;
        }

        public FloorRequest Dequeue()
        {
            FloorRequest request = Peek();
            Remove(request);

            _log.Debug($"Dequeued request {request} from front of queue. The queue currently holds {_queue.Count} requests.");

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

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable)_queue).GetEnumerator();
        }

        IEnumerator<FloorRequest> IEnumerable<FloorRequest>.GetEnumerator()
        {
            return ((IEnumerable<FloorRequest>)_queue).GetEnumerator();
        }
    }
}