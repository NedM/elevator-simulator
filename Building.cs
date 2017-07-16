using System;
using System.Collections.Generic;

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
                _floors.Add(i, new Floor(i));
            }
        }

        public Floor GetFloor(int floorNumber)
        {
            if (floorNumber > _numberOfFloors || floorNumber < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return _floors[floorNumber];
        }
    }
}