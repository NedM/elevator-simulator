using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Elevator
{
    public class Elevator : IElevator
    {
        private enum Status { Idle, Running }

        private const int ELEVATOR_SPEED = 2000; // Number of milliseconds required to move between floors

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
                        if (_status != Status.Idle)
                        {
                            _log.Info($"Elevator {Id} is becoming idle!");
                        }

                        _status = Status.Idle;
                        Thread.Sleep(100);
                        continue;
                    }

                    if (_status != Status.Running)
                    {
                        _log.Info($"Elevator {Id} is becoming active!");
                    }

                    _status = Status.Running;
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
            RequestFloor(new FloorRequest(floorNumber, direction));            
        }

        public void RequestFloor(FloorRequest request)
        {
            AddFloorRequest(request);
        }

        private void AddFloorRequest(FloorRequest request)
        {
            _log.Info($"Elevator {Id} received floor request {request}.");

            StringBuilder sb = new StringBuilder($"Adding Floor Request: {request} to ");
            string queueContents;

            switch (DetermineRequestQueue(request))
            {
                case Direction.Up:
                    _upDirectionQueue.Add(request);                    

                    sb.Append("up direction queue");
                    queueContents = _upDirectionQueue.ToString();
                    break;
                case Direction.Down:
                    _downDirectionQueue.Add(request);

                    sb.Append("down direction queue");
                    queueContents = _downDirectionQueue.ToString();
                    break;
                default:
                    throw new InvalidOperationException("Invalid direction request queue!");
            }

            sb.Append($" for elevator {Id}. Queue contains: {queueContents}.");

            _log.Debug(sb.ToString());
        }

        private Direction DetermineRequestQueue(FloorRequest request)
        {
            if(request.Direction == Direction.None)
            {
                return request.Floor > CurrentFloor ? Direction.Up : Direction.Down;
            }

            return request.Direction;
        }

        private void Ascend()
        {
            if (CurrentFloor < HighestFloor)
            {
                MoveElevator(Direction.Up);
            }
        }

        private void CycleDoors(Floor floor)
        {
            _log.Info($"Elevator {Id} opening doors on {floor}....");
            Thread.Sleep(1000); //Simulate time to stop at floor and open doors

            Random rand = new Random(DateTime.Now.Millisecond);

            Thread.Sleep(rand.Next(3000, 10000)); //Simulate time to open doors and admit passengers
            _log.Info($"Elevator {Id} closing doors on {floor}...");

            do
            {
                Thread.Sleep(rand.Next(1000, 3000)); //Simulate time to close doors including any last minute additional passengers arriving
            } while (DoorsHeld());

            _log.Info($"Elevator {Id} doors closed on {floor}...");
        }

        private void Descend()
        {
            if (CurrentFloor > LowestFloor)
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
                _log.Info($"Elevator {Id} received doors hold command! Holding doors...");
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

            _log.Debug($"Moving elevator {Id} {dirString} 1 floor.");
            Thread.Sleep(ELEVATOR_SPEED);  //Simulate time required to move between floors
            _log.Info($"Elevator {Id} is now at {CurrentFloor}.");
        }

        private IEnumerable<FloorRequest> FloorsAboveCurrent(FloorRequestQueue queue)
        {
            return queue.Any ? queue.Where(f => f.Floor >= CurrentFloor) : null;
        }

        private IEnumerable<FloorRequest> FloorsBelowCurrent(FloorRequestQueue queue)
        {
            return queue.Any ? queue.Where(f => f.Floor <= CurrentFloor) : null;
        }

        private FloorRequest GetNextRequestAbove()
        {
            var nextRequestAbove = FloorsAboveCurrent(_upDirectionQueue)?.FirstOrDefault();

            if (null == nextRequestAbove)
            {
                FloorRequest highestDownRequest = _downDirectionQueue.Peek();

                nextRequestAbove = (highestDownRequest?.Floor >= CurrentFloor) ? highestDownRequest : null;
            }

            return nextRequestAbove;
        }

        private FloorRequest GetNextRequestBelow()
        {
            FloorRequest nextRequestBelow = FloorsBelowCurrent(_downDirectionQueue)?.FirstOrDefault();

            if(null == nextRequestBelow)
            {
                FloorRequest lowestUpRequest = _upDirectionQueue.Peek();

                nextRequestBelow = (lowestUpRequest?.Floor <= CurrentFloor) ? lowestUpRequest : null;
            }

            return nextRequestBelow;
        }

        private void ReverseDirectionOfTravel()
        {
            StringBuilder sb = new StringBuilder($"Changing elevator {Id} direction from {Enum.GetName(typeof(Direction), DirectionOfTravel)}");
            DirectionOfTravel = DirectionOfTravel == Direction.Up 
                ? Direction.Down 
                : Direction.Up;

            sb.Append($" to {Enum.GetName(typeof(Direction), DirectionOfTravel)}.");
            _log.Debug(sb.ToString());
        }

        private void ServiceFloorRequests()
        {
            switch (DirectionOfTravel)
            {
                case Direction.Down:
                    ServiceFloorRequestsInDownwardDirection();
                    break;
                case Direction.Up:
                default:
                    ServiceFloorRequestInUpwardDirection();
                    break;
            }
        }

        private void ServiceFloorRequestsInDownwardDirection()
        {
            FloorRequest nextRequestBelow = GetNextRequestBelow();

            if (null == nextRequestBelow)
            {
                return;
            }

            if (nextRequestBelow.Floor != CurrentFloor)
            {
                Descend();
                return;
            }

            if (nextRequestBelow.Direction != Direction.Up)
            {
                Floor stopFloor = nextRequestBelow.Floor;

                _downDirectionQueue.Remove(nextRequestBelow);

                StopAtFloor(stopFloor);
            }

            if (ShouldReverseDirections())
            {
                ReverseDirectionOfTravel();
            }
        }

        private void ServiceFloorRequestInUpwardDirection()
        {
            FloorRequest nextRequestAbove = GetNextRequestAbove();

            if (null == nextRequestAbove)
            {
                return;
            }

            if (nextRequestAbove.Floor != CurrentFloor)
            {
                Ascend();
                return;
            }

            if (nextRequestAbove.Direction != Direction.Down)
            {
                Floor stopFloor = nextRequestAbove.Floor;

                _upDirectionQueue.Remove(nextRequestAbove);

                StopAtFloor(stopFloor);
            }

            if (ShouldReverseDirections())
            {
                ReverseDirectionOfTravel();
            }
        }

        private bool ShouldReverseDirections()
        {
            bool shouldReverse = false;

            switch (DirectionOfTravel)
            {
                case Direction.Up:
                    if(CurrentFloor == HighestFloor)
                    {
                        shouldReverse = true;
                    }
                    else
                    {
                        var nextRequestAbove = GetNextRequestAbove();

                        if (null == nextRequestAbove || nextRequestAbove.Floor == CurrentFloor)
                        {
                            shouldReverse = true;
                        }
                    }                    
                    break;
                case Direction.Down:
                    if (CurrentFloor == LowestFloor)
                    {
                        shouldReverse = true;
                    }
                    else
                    {
                        var nextRequestBelow = GetNextRequestBelow();

                        if (null == nextRequestBelow || nextRequestBelow.Floor == CurrentFloor)
                        {
                            shouldReverse = true;
                        }                        
                    }
                    break;
                default:
                    break;
            }
            
            return shouldReverse;
        }

        private void StopAtFloor(Floor floor)
        {            
            _log.Info($"Elevator {Id} is stopping at {floor}...");
            
            CycleDoors(floor);
        }
    }
}