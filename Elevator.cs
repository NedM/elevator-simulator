using System;
using System.Collections;
using System.Collections.Generic;
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

        public Elevator(int id, Floor highestFloor, Floor lowestFloor)
        {
            //IComparer<int> ascending = new Comparison<int>((a, b) => a.CompareTo(b));
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

        public Floor CurrentFloor { get; private set; }
        public Direction DirectionOfTravel { get; private set; }
        public Floor HighestFloor { get; private set; }
        public int Id { get; private set;}
        public Floor LowestFloor { get; private set; }

        public void Run()
        {
            _stop = false;
            _runThread = new Thread(() =>
            {
                while (!_stop)
                {
                    if(!_upDirectionQueue.Any && !_downDirectionQueue.Any)
                    {
                        _status = Status.Idle;
                        DirectionOfTravel = Direction.None;
                        Thread.Sleep(50);
                        continue;
                    }
                    
                    if(DirectionOfTravel == Direction.Up)
                    {
                        if (_upDirectionQueue.Any)
                        {

                        }
                    }

                    
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
            if(floor == CurrentFloor)
            {
                return;
            }

            AddFloorRequest(new FloorRequest(floor, Direction.None));
        }

        public void RequestFloor(int floorNumber)
        {
            RequestFloor(new Floor(floorNumber));
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
            return request.Floor.CompareTo(CurrentFloor) < 0 && 
                request.Direction != Direction.Up && 
                DirectionOfTravel != Direction.Up;
        }

        private bool IsOnTheWayUp(FloorRequest request)
        {
            return request.Floor.CompareTo(CurrentFloor) >= 0 && 
                request.Direction != Direction.Down &&
                DirectionOfTravel != Direction.Down;
        }
    }
}