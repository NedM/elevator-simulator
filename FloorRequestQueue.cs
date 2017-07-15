namespace Elevator
{
    public class FloorRequestQueue
    {
        private Queue<FloorRequest> _queue;

        public FloorRequestQueue()
        {
            _queue = new Queue<FloorRequest>();
        }

        public void Add(FloorRequest request)
        {
            if(!_queue.Contains(request))
            {
                _queue.Add(request);
                _queue.Sort();
            }
        }

        public void Remove(FloorRequest request)
        {
            _queue.Remove(request);
        }

        public override string ToString()
        {
            return $"[{_queue.join(, )}]";
        }
    }
}