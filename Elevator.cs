using System;
using System.Threading;

namespace Elevator
{
    public class Elevator : IElevator
    {
        private enum Status { Idle, Running }

        private FloorRequestQueue _upDirectionQueue;
        private FloorRequestQueue _downDirectionQueue;
        private volatile bool _stop;
        private Status _status;
        private Thread _runThread;

        public Elevator(int id, int highestFloor, int lowestFloor)
        {
            _upDirectionQueue = new FloorRequestQueue();
            _downDirectionQueue = new FloorRequestQueue();
            _stop = true;

            DirectionOfTravel = Direction.None;
            CurrentFloor = lowestFloor;
            HighestFloor = highestFloor;
            Id = id;
            LowestFloor = lowestFloor;
            _status = Status.Idle;
        }

        public int CurrentFloor { get; private set; }
        public Direction DirectionOfTravel { get; private set; }
        public int HighestFloor { get; private set; }
        public int Id { get; private set;}
        public int LowestFloor { get; private set; }

        public void Run()
        {
            _stop = false;
            _runThread = new Thread(() =>
            {
                while (!_stop)
                {

                }
            })
            {
                IsBackground = true,
            };

            _runThread.Start();
        }

        public void Stop()
        {
            _stop = true;
            _runThread.Join();
            _runThread = null;
        }

        public void RequestFloor(Floor floor)
        {
            RequestFloor(floor.Number);
        }

        public void RequestFloor(int floorNumber)
        {
            if(floorNumber == CurrentFloor)
            {
                return;
            }

            AddFloorRequest(new FloorRequest(floorNumber, Direction.None));
        }

        public void RequestElevator(Floor floor, Direction direction)
        {
            AddFloorRequest(new FloorRequest(floor.Number, direction));
        }

        private void AddFloorRequest(FloorRequest request)
        {
            if (IsOnTheWayDown(request))
            {
                _downDirectionQueue.Add(request);
            }
            else
            {
                _upDirectionQueue.Add(request);
            }
            // switch(request.Direction)
            // {
            //     case Direction.Down:
            //         IsOnTheWay(request) ? _downDirectionQueue.Add(request) : _upDirectionQueue.Add(request);
            //     case Direction.Up:
            //         IsOnTheWay(request) ? _upDirectionQueue.Add(request) : _downDirectionQueue.Add(request);
            //     case Direction.None:
            //     default:
                    
            // }
        }

        private bool IsIdle { get { return _status == Status.Idle; } }

        //private bool IsOnTheWay(FloorRequest request)
        //{
        //    return IsOnTheWayDown(request) || IsOnTheWayUp(request);
        //}

        private bool IsOnTheWayDown(FloorRequest request)
        {
            return request.Floor < CurrentFloor && 
                request.Direction != Direction.Up && 
                DirectionOfTravel != Direction.Up;
        }

        private bool IsOnTheWayUp(FloorRequest request)
        {
            return request.Floor >= CurrentFloor && 
                request.Direction != Direction.Down &&
                DirectionOfTravel != Direction.Down;
        }
    }
}