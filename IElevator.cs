namespace Elevator
{
    public interface IElevator
    {
        Floor CurrentFloor { get; }
        Direction DirectionOfTravel { get; }
        Floor HighestFloor { get; }
        int Id { get; }
        bool IsIdle { get; }
        Floor LowestFloor { get; }
        void RequestFloor(int floorNumber, Direction direction);
        void RequestFloor(FloorRequest request);
        void Run();
        void Stop();
    }
}