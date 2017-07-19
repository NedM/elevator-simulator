using System;

namespace Elevator
{
    public class FloorRequest : IComparable
    {
        public FloorRequest(Floor floor, Direction direction = Direction.None)
        {
            Floor = floor;
            Direction = direction;
        }

        public Floor Floor { get; private set; }

        public Direction Direction { get; private set; }

        public int CompareTo(object obj)
        {
            if(obj == null)
            {
                return 1;
            }

            FloorRequest fr = obj as FloorRequest;

            if (fr != null)
            {
                return CompareTo(fr);
            }

            throw new ArgumentException($"Object with type {obj.GetType().Name} is not a FloorRequest!");
        }

        public int CompareTo(FloorRequest other)
        {
            return this.Floor.CompareTo(other.Floor);
        }

        public override string ToString()
        {
            return $"({Floor}, {Enum.GetName(typeof(Direction), Direction)})";
        }
    }
}