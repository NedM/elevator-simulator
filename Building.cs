namespace Elevator
{
    public class Building
    {
        private readonly int _numberOfFloors;
        private Dictionary<int, Floor> _floors;

        public Building(int numberOfFloors)
        {
            if(numberOfFloors < 1)
            {
                throw new ArgumentOutOfRangeException();
            }

            _numberOfFloors = numberOfFloors;
            _floors = new Dictionary<int, Floor>(_numberOfFloors);

            for(int i = 0; i < _numberOfFloors; i++)
            {
                _floors.add(i, new Floor(i));
            }

            public Floor GetFloor(int floorNumber)
            {
                if(floorNumber > _numberOfFloors || floorNumber < 0)
                {
                    throw new ArgumentOutOfRangeException();
                }

                return _floors[floorNumber];
            }
        }
    }

    public class Floor
    {
        private readonly int _floorNumber;
        public Floor(int floorNumber)
        {
            _floorNumber = floorNumber;
        }

        public int Number { get { return _floorNumber} }

        public FloorRequest RequestElevator(Direction direction)
        {
            return new FloorRequest(Number, direction);
        }
    }
}