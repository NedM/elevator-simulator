namespace Elevator
{
    public interface IElevator
    {
        int Id { get; }
        void Run();
        void Stop();
        void RequestFloor(Floor floor);
        void RequestFloor(int floorNumber);
        void RequestElevator(Floor floor, Direction direction);
    }
}