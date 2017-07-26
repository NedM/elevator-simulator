using log4net;
using System;
using System.Linq;

namespace Elevator
{
    public class ElevatorSystem : IElevatorSystem
    {
        private readonly IElevator[] _elevators;
        private readonly ILog _log;

        public ElevatorSystem(IElevator[] elevators)
        {
            _elevators = elevators;
            _log = LogManager.GetLogger(typeof(ElevatorSystem));
        }

        public int[] Elevators { get { return _elevators.Select(e => e.Id).ToArray(); } }
        public Floor HighestFloorServiced { get { return _elevators.Max(e => e.HighestFloor); } }
        public Floor LowestFloorServiced { get { return _elevators.Min(e => e.LowestFloor); } }

        public IElevator GetElevator(int elevatorId)
        {
            return _elevators.Where(e => e.Id == elevatorId).First();
        }

        public int RequestElevator(int floorNumber, Direction direction)
        {
            return RequestElevator(new FloorRequest(new Floor(floorNumber), direction));
        }

        public int RequestElevator(FloorRequest request)
        {
            //TODO: Use extension method on IElevator[]
            //TODO: Find nearest elevator moving in the correct direction and request floor on that elevator

            // Simple algorithm: Find first idle elevator or first one traveling in the desired direction and dispatch request. 
            // If none meet these requirements, pick one at random.
            // Future possible optimizations:
            //   Find nearest elevator moving in the correct direction and request floor on that elevator
            //   Find least busy elevator and dispatch request

            //var simple = _elevators.Where(e => e.IsIdle);
            //var result = _elevators.Where(e => (e.IsIdle || (e.DirectionOfTravel == request.Direction)));
            var firstIdleElevator = _elevators.FirstOrDefault(e => (e.IsIdle || (e.DirectionOfTravel == request.Direction)));

            IElevator target = firstIdleElevator != null ? firstIdleElevator : GetRandomElevator();

            _log.Info($"Dispatching floor request {request} to elevator {target.Id}.");

            target.RequestFloor(request);

            return target.Id;
        }

        public void RunAll()
        {
            foreach (IElevator elevator in _elevators)
            {
                elevator.Run();
            }
        }

        public void Run(int elevatorId)
        {
            GetElevator(elevatorId).Run();
        }

        public void StopAll()
        {
            foreach (IElevator elevator in _elevators)
            {
                elevator.Stop();
            }
        }

        public void Stop(int elevatorId)
        {
            GetElevator(elevatorId).Stop();
        }

        private IElevator GetRandomElevator()
        {
            Random rand = new Random(DateTime.Now.Millisecond);

            return _elevators[rand.Next(_elevators.Length)];
        }
    }
}