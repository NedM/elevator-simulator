using log4net;
using System;
using System.Text;
using System.Threading;

namespace Elevator
{
    public class Elevator : IElevator
    {
        private enum Status { Idle, Running }

        private const int ELEVATOR_SPEED = 1000; // Number of milliseconds required to move between floors

        private static object sync_lock = new object();

        private static Comparison<FloorRequest> ascending = (a, b) => a.CompareTo(b);
        private static Comparison<FloorRequest> descending = (a, b) => b.CompareTo(a);

        private FloorRequestQueue _upDirectionQueue;
        private FloorRequestQueue _downDirectionQueue;

        private volatile bool _stop;
        private Status _status;
        private Thread _runThread;
        private ILog _log = LogManager.GetLogger(typeof(ElevatorSystem));

        public Elevator(int id, Floor highestFloor, Floor lowestFloor)
        {
            _upDirectionQueue = new FloorRequestQueue(ascending);
            _downDirectionQueue = new FloorRequestQueue(descending);
            _stop = true;

            DirectionOfTravel = Direction.Up;
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
        public bool IsIdle { get { return _status == Status.Idle; } }
        public Floor LowestFloor { get; private set; }

        public void Run()
        {
            _stop = false;
            _runThread = new Thread(() =>
            {
                _log.Info($"Elevator {Id} is starting up!");

                while (!_stop)
                {
                    if (!_upDirectionQueue.Any && !_downDirectionQueue.Any)
                    {
                        //_log.Debug($"No requests to service for elevator {Id}.");
                        _status = Status.Idle;
                        Thread.Sleep(1000);
                        continue;
                    }

                    _status = Status.Running;
                    _log.Debug($"Elevator {Id} is running!");
                    ServiceFloorRequests();
                }

                _log.Info($"Elevator {Id} is stopped!");
            })
            {
                IsBackground = true,
            };

            _log.Info($"Starting Elevator {Id}...");
            _runThread.Start();
        }

        public void Stop()
        {
            _log.Info($"Stopping Elevator {Id}...");
            _stop = true;
            _runThread.Join();
            _runThread = null;
        }

        public void RequestFloor(int floorNumber, Direction direction = Direction.None)
        {
            if (floorNumber == CurrentFloor.Number)
            {
                return;
            }

            AddFloorRequest(new FloorRequest(floorNumber, direction));
        }

        public void RequestFloor(FloorRequest request)
        {
            if(request.Floor == CurrentFloor)
            {
                return;
            }

            AddFloorRequest(request);
        }

        private void AddFloorRequest(FloorRequest request)
        {
            _log.Info($"Elevator {Id} received floor request {request}.");

            StringBuilder sb = new StringBuilder($"Adding Floor Request: {request} to ");

            switch (DetermineRequestQueue(request))
            {
                case Direction.Up:
                    lock (sync_lock)
                    {
                        _upDirectionQueue.Add(request);
                    }

                    sb.Append("up direction queue");
                    break;
                case Direction.Down:
                    lock (sync_lock)
                    {
                        _downDirectionQueue.Add(request);
                    }

                    sb.Append("down direction queue");
                    break;
                default:
                    throw new InvalidOperationException("Invalid direction request queue!");
            }

            sb.Append($" for elevator {Id}.");

            _log.Debug(sb.ToString());
        }

        private Direction DetermineRequestQueue(FloorRequest request)
        {
            if(request.Direction == Direction.None)
            {
                return request.Floor.CompareTo(CurrentFloor) > 0 ? Direction.Up : Direction.Down;
            }

            return request.Direction;
        }

        private void Ascend()
        {
            if (CurrentFloor.CompareTo(HighestFloor) < 0)
            {
                MoveElevator(Direction.Up);
            }
        }

        private void CycleDoors(Floor floor)
        {
            _log.Info($"Elevator {Id} opening doors on floor {floor}.");
            Thread.Sleep(1000); //Simulate time to stop at floor and open doors

            Random rand = new Random(DateTime.Now.Millisecond);

            Thread.Sleep(rand.Next(3000, 10000)); //Simulate time to open doors and admit passengers
            _log.Info($"Elevator {Id} closing doors on floor {floor}...");

            do
            {
                Thread.Sleep(rand.Next(1000, 3000)); //Simulate time to close doors including any last minute additional passengers arriving
            } while (DoorsHeld());

            _log.Info($"Elevator {Id} doors closed on floor {floor}...");
        }

        private void Descend()
        {
            if (CurrentFloor.CompareTo(LowestFloor) > 0)
            {
                MoveElevator(Direction.Down);
            }
        }

        private bool DoorsHeld()
        {
            Random doorsHeldChance = new Random(DateTime.Now.Millisecond);

            // Use Monte Carlo method to simulate 1 in 10 chance that doors hold button is pressed
            bool doorsHold = doorsHeldChance.Next(10) == 1;

            if (doorsHold)
            {
                _log.Info("Elevator {Id} received doors hold command! Holding doors...");
            }

            return doorsHold;
        }

        private void MoveElevator(Direction direction)
        {
            string dirString = null;

            switch (direction)
            {
                case Direction.Down:
                    CurrentFloor--;
                    dirString = "down";
                    break;
                case Direction.Up:
                    CurrentFloor++;
                    dirString = "up";
                    break;
                default:
                    break;
            }

            _log.Info($"Moving elevator {Id} {dirString} 1 floor.");
            Thread.Sleep(ELEVATOR_SPEED);  //Simulate time required to move between floors
            _log.Info($"Elevator {Id} is now at floor {CurrentFloor}.");
        }

        private bool NextRequestedFloorAbove(FloorRequestQueue queue)
        {
            return queue.Any && queue.Peek().Floor.CompareTo(CurrentFloor) >= 0;
        }

        private bool NextRequestedFloorBelow(FloorRequestQueue queue)
        {
            return queue.Any && queue.Peek().Floor.CompareTo(CurrentFloor) <= 0;
        }

        private void ReverseDirectionOfTravel()
        {
            DirectionOfTravel = DirectionOfTravel == Direction.Up 
                ? Direction.Down 
                : Direction.Up;
        }

        private void ServiceFloorRequests()
        {
            switch (DirectionOfTravel)
            {
                case Direction.Down:
                    if (_downDirectionQueue.Any)
                    {
                        if (NextRequestedFloorBelow(_downDirectionQueue))
                        {
                            ServiceFloorRequestsInDownwardDirection();
                        }
                        else
                        {
                            Ascend();
                        }
                    }

                    break;
                case Direction.Up:
                default:
                    if (_upDirectionQueue.Any)
                    {
                        if (NextRequestedFloorAbove(_upDirectionQueue))
                        {
                            ServiceFloorRequestInUpwardDirection();
                        }
                        else
                        {
                            Descend();
                        }
                    }

                    break;
            }

            if (ShouldReverseDirections())
            {
                ReverseDirectionOfTravel();
            }
        }

        private void ServiceFloorRequestsInDownwardDirection()
        {
            if (ShouldStop(_downDirectionQueue))
            {
                FloorRequest stopFloor = null;
                lock (sync_lock)
                {
                    stopFloor = _downDirectionQueue.Dequeue();
                }

                StopAtFloor(stopFloor);
            }
            
            if(NextRequestedFloorBelow(_downDirectionQueue))
            {
                Descend();
            }
        }

        private void ServiceFloorRequestInUpwardDirection()
        {
            if (ShouldStop(_upDirectionQueue))
            {
                FloorRequest stopFloor = null;

                lock (sync_lock)
                {
                    stopFloor = _upDirectionQueue.Dequeue();
                }

                StopAtFloor(stopFloor);
            }

            if(NextRequestedFloorAbove(_upDirectionQueue))
            {
                Ascend();
            }
        }

        private bool ShouldReverseDirections()
        {
            if (DirectionOfTravel == Direction.Up)
            {
                return CurrentFloor == HighestFloor || !_upDirectionQueue.Any; // || NextRequestedFloorBelow(_upDirectionQueue);
            }

            if (DirectionOfTravel == Direction.Down)
            {
                return CurrentFloor == LowestFloor || !_downDirectionQueue.Any; // || NextRequestedFloorAbove(_downDirectionQueue);
            }

            return false;
        }

        private bool ShouldStop(FloorRequestQueue queue)
        {
            return queue.Any && queue.Peek().Floor == CurrentFloor;
        }

        private void StopAtFloor(FloorRequest floorRequest)
        {            
            _log.Info($"Elevator {Id} is stopping at floor {floorRequest.Floor}...");
            
            CycleDoors(floorRequest.Floor);
        }
    }
}