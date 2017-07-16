using System.Collections.Generic;

namespace Elevator
{
    public class FloorRequestQueue
    {
        private SortedList<int, Direction> _queue;

        public FloorRequestQueue()
        {
            _queue = new SortedList<int, Direction>();
        }

        public void Add(FloorRequest request)
        {
            if(!_queue.ContainsKey(request.Floor))
            {
                _queue.Add(request.Floor, request.Direction);
            }
        }

        public void Remove(FloorRequest request)
        {
            _queue.Remove(request.Floor);
        }

        public void RemoveHighest()
        {
            _queue.RemoveAt(_queue.Count - 1);
        }

        public void RemoveLowest()
        {
            _queue.RemoveAt(0);
        }

        public override string ToString()
        {
            return $"[{string.Join(", ", _queue)}]";
        }
    }
}