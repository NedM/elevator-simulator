namespace Elevator
{
    public class ElevatorFactory
    {
        private static int _elevatorId;

        private static ElevatorFactory _instance = null;
        public static Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ElevatorFactory();
                }

                return _instance;
            }}

        private ElevatorFactory()
        {
            _elevatorId = 0;
        }

        public IElevator Create(int highestFloor = 10, int lowestFloor = 0)
        {
            return new Elevator(++_elevatorId, highestFloor, lowestFloor);
        }
    }
}