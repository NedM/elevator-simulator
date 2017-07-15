namespace Elevator
{
    public class FloorRequest
    {
        protected FloorRequest(int floor, Direction direction = Direction.None)
        {
            Floor = floor;
            Direction = direction;
        }

        public int Floor { get; private set; }

        public Direction Direction { get; private set; }

        public override string ToString()
        {
            return $"({Floor}, {enum.GetName(typeof(Direction), Direction})";
        }
    }
}