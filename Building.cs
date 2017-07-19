using System;
using System.Collections.Generic;

namespace Elevator
{
    public class Building
    {
        private Dictionary<int, Floor> _floors;
        
        //TODO: Add Elevator System
        
        //TODO: Add concept of elevator banks that service different floors?

        public Building(int numberOfFloors)
        {
            if (numberOfFloors < 1)
            {
                throw new ArgumentOutOfRangeException("A building must have at least 1 floor.");
            }

            Initialize(0, numberOfFloors);
        }

        public Building(int lowestFloor, int highestFloor)
        {
            if(lowestFloor >= highestFloor)
            {
                throw new ArgumentException($"Lowest floor ({lowestFloor}) must be lower than highest floor ({highestFloor})!");
            }

            Initialize(lowestFloor, highestFloor);
        }

        public Building(IReadOnlyCollection<Floor> floors)
        {
            _floors = new Dictionary<int, Floor>(floors.Count);

            foreach(Floor f in floors)
            {
                if (_floors.ContainsKey(f.Number))
                {
                    throw new ArgumentException($"Duplicated floor entry: {_floors[f.Number]} & {f}.");
                }

                _floors.Add(f.Number, f);
            }
        }

        public Floor GetFloor(int floorNumber)
        {
            return _floors.ContainsKey(floorNumber) ? _floors[floorNumber] : null;
        }

        private void Initialize(int lowestFloor, int highestFloor)
        {
            _floors = new Dictionary<int, Floor>();

            for (int i = lowestFloor; i < highestFloor; i++)
            {
                _floors.Add(i, new Floor(i));
            }
        }

        //public void RequestElevator(IRequestElevator requestor, Direction direction)
        //{
        //    //Get best elevator to service request then add floor request to the queue
        //    AddFloorRequest(new FloorRequest(requestor.Floor, direction));
        //}
    }
}