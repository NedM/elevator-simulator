using System;

namespace Elevator
{
    public class Elevator : IElevator
    {
        private enum Status { Idle, Running }
        private Queue<FloorRequest> _upDirectionQueue;
        private Queue<FloorRequest> _downDirectionQueue;

        public Elevator(int id, int highestFloor, int lowestFloor)
        {
            _upDirectionQueue = new Queue<FloorRequest>();
            _downDirectionQueue = new Queue<FloorRequest>();

            DirectionOfTravel = Direction.None;
            CurrentFloor = lowestFloor;
            HighestFloor = highestFloor;
            Id = id;
            LowestFloor = lowestFloor;
        }

        public int CurrentFloor { get; private set; }
        public Direction DirectionOfTravel { get; private set; }
        public int HighestFloor { get; private set; }
        public int Id { get; private set;}
        public int LowestFloor { get; private set; }

        public void Run()
        {
            throw new NotImplementedException();
        }

        public void RequestFloor(int floorNumber)
        {
            if(Status == Status.Idle)
            {
                if(floorNumber > CurrentFloor)
                {
                    _upDirectionQueue.add()
                }
            }
            if(DirectionOfTravel == Direction.Up)
        }

        public void AddFloorRequest(FloorRequest request)
        {
            if(request.Floor > CurrentFloor && )
            {
                _upDirectionQueue.add(request)
            }
        }
    }
}