namespace Elevator
{
    public interface IElevator
    {
        int CurrentFloor { get; }
        Direction DirectionOfTravel { get; }
        int HighestFloor { get; }
        int Id { get; }
        bool IsIdle { get; }
        int LowestFloor { get; }
        void RequestFloor(int floorNumber, Direction direction);
        void RequestFloor(FloorRequest request);
        void Run();
        void Stop();
    }
}