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
    }
}