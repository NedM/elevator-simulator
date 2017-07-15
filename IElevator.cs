namespace Elevator
{
    public interface IElevator
    {
        int Id { get; }
        void Run();
        void AddFloorRequest(FloorRequest request);
    }
}