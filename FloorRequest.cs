using System;

namespace Elevator
{
    public class FloorRequest : IComparable
    {

        public FloorRequest(int floorNumber, Direction direction = Direction.None) : this(new Floor(floorNumber), direction)
        {
        }

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
            var compareResult = Floor.CompareTo(other.Floor);

            if (compareResult == 0 && Direction != Direction.None && other.Direction != Direction.None)
            {
                compareResult = Direction.CompareTo(other.Direction);
            }

            return compareResult;
        }

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return ReferenceEquals(this, obj);
            }

            FloorRequest request = obj as FloorRequest;

            if(null != request)
            {
                return Equals(request);
            }

            return false;
        }

        public bool Equals(FloorRequest other)
        {
            return CompareTo(other) == 0;
        }

        public override int GetHashCode()
        {
            return Floor.GetHashCode() + Direction.GetHashCode();
        }

        public override string ToString()
        {
            return $"({Floor}, {Enum.GetName(typeof(Direction), Direction)})";
        }
    }
}