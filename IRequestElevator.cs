namespace Elevator
{
    public interface IRequestElevator
    {
        int RequestElevator(int floorNumber, Direction direction);
        int RequestElevator(FloorRequest request);
    }
}
