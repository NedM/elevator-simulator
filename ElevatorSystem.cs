using System.Threading;

namespace Elevator
{
    public class ElevatorSystem
    {
        private IElevator[] _elevators;

        public ElevatorSystem(IElevator[] elevators)
        {
            _elevators = elevators;
        }

        public void RunAll()
        {
            foreach (IElevator elevator in _elevators)
            {
                elevator.Run();
            }
        }

        public void StopAll()
        {
            foreach (IElevator elevator in _elevators)
            {
                elevator.Stop();
            }
        }
    }
}