using System;
using System.Linq;

namespace Elevator
{
    public class ElevatorFactory
    {
        private static int _elevatorId;

        private static ElevatorFactory _instance = null;
        public static ElevatorFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ElevatorFactory();
                }

                return _instance;
            }
        }

        private ElevatorFactory()
        {
            _elevatorId = 0;
        }

        public IElevator Create(int highestFloor = 10, int lowestFloor = 0)
        {
            return new Elevator(++_elevatorId, new Floor(highestFloor), new Floor(lowestFloor));
        }

        public IElevator Create(Floor[] floors)
        {
            if(floors.GroupBy(f => f.Number).Any(g => g.Count() > 1))
            {
                throw new ArgumentException("Duplicate floor number detected! Floor numbers must be unique!");
            }

            return Create(floors.Max(f => f.Number), floors.Min(f => f.Number)); //TODO: Invert this so Elevator Constructor takes a collection of floors it services
        }
    }
}