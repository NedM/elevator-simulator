using System;
using System.Collections.Generic;
using System.Text;

namespace Elevator
{ 
    interface IElevatorSystem : IRequestElevator
    {
        IElevator GetElevator(int elevatorId);
        void Run(int elevatorId);
        void RunAll();
        void Stop(int elevatorId);
        void StopAll();
    }
}
