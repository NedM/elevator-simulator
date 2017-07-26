using System;
using System.Collections.Generic;

namespace Elevator
{
    public class Building
    {
        private static Building _instance;

        public static void Initialize(ElevatorSystem system)
        {
            _instance = new Building(system);
        }

        public static Building Instance
        {
            get
            {
                if (null == _instance)
                {
                    throw new InvalidOperationException("Must initialize before use!");
                }

                return _instance;
            }
        }

        private readonly Dictionary<int, Floor> _floors;

        //TODO: Add concept of elevator banks that service different floors

        private Building(ElevatorSystem system)
        {
            _floors = new Dictionary<int, Floor>();

            for (int i = system.LowestFloorServiced.Number; i <= system.HighestFloorServiced.Number; i++)
            {
                _floors.Add(i, new Floor(i));
            }
        }

        public Floor GetFloor(int floorNumber)
        {
            return _floors.ContainsKey(floorNumber) ? _floors[floorNumber] : null;
        }
    }
}