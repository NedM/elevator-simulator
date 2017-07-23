using System;

namespace Elevator
{
    public class Floor : IComparable, IComparable<Floor>, IRequestElevator
    {
        private readonly ElevatorSystem _elevators;
        private readonly string _name;
        private readonly int _number;

        //TODO: Restrict to collection of elevators serving this floor

        public Floor(ElevatorSystem elevatorSystem, int number, string name = null)
        {
            if(elevatorSystem.HighestFloorServiced < number || elevatorSystem.LowestFloorServiced > number)
            {
                throw new ArgumentException($"No elevators in the system can serve this floor! This floor is number {number}. " +
                    $"The lowest floor served is {elevatorSystem.LowestFloorServiced} and the highest floor served is {elevatorSystem.HighestFloorServiced}.");
            }

            _elevators = elevatorSystem;
            _number = number;
            _name = name;
        }

        public int Number { get { return _number; } }

        public string Name { get { return _name; } }

        public int CompareTo(object obj)
        {
            if(null == obj)
            {
                return 1;
            }

            Floor floor = obj as Floor;

            if(null != floor)
            {
                return CompareTo(floor);
            }

            throw new ArgumentException($"Object with type {obj.GetType().Name} is not a Floor!");
        }

        public int CompareTo(Floor other)
        {
            return null == other ? 1 : Number.CompareTo(other.Number);
        }

        public int RequestElevator(Direction direction)
        {
            return _elevators.RequestElevator(new FloorRequest(this, direction));
        }

        public override int GetHashCode()
        {
            return Number.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            Floor floor = obj as Floor;

            if(null != floor)
            {
                return Equals(floor);
            }

            return base.Equals(obj);
        }

        public bool Equals(Floor other)
        {
            return null != other && Number.Equals(other.Number);
        }

        public override string ToString()
        {
            return string.Format("Floor {0}{1}", Number, 
                (Name != null && Name.Length > 0) 
                    ? " - " + Name 
                    : string.Empty);
        }
    }
}