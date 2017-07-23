using Elevator;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace ElevatorSimulator
{
    class Program
    {
        private const string EXIT_COMMAND = "exit";

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            ElevatorSystem system = new ElevatorSystem(new Elevator.IElevator[]{
                ElevatorFactory.Instance.Create(),
            });

            Building.Initialize(system);

            system.RunAll();

            string userSelection = null;
            Console.WriteLine("Welcome to the Elevator System! " + Environment.NewLine +
                "Please enter selections at the prompts to enter floor requests. " + Environment.NewLine +
                "Type \"exit\" at the prompt to terminate the program.");

            while (true)
            {
                string response = PromptForInput("Are you already in the elevator?", new string[] { "y", "n", "exit" });

                if (CheckForExitCommand(response))
                {
                    break;
                }
                else if (response.CompareTo("n") == 0)
                {
                    HandleOutsideElevator(system, Building.Instance);
                }
                else
                {
                    HandleInsideElevator(system, Building.Instance);
                }

                Thread.Sleep(2500);
            }

            system.StopAll();
        }

        private static bool CheckForExitCommand(string command)
        {
            return command.CompareTo(EXIT_COMMAND) == 0;
        }

        private static void HandleInsideElevator(ElevatorSystem system, Building building)
        {
            string response = PromptForInput("Which elevator are you in?", system.Elevators.Select(i => i.ToString()).Concat(new string[] { "exit" }).ToArray());

            if (CheckForExitCommand(response))
            {
                return;
            }

            int elevatorId = int.Parse(response);

            Console.WriteLine("What floor do you want to go to?");
            int floorNum = 0;

            do
            {
                Console.WriteLine($"[{system.LowestFloorServiced} - {system.HighestFloorServiced}] Floor Number:> ");
                response = Console.ReadLine();
                if (CheckForExitCommand(response))
                {
                    return;
                }
            } while (!int.TryParse(response, out floorNum) && floorNum <= system.HighestFloorServiced && floorNum >= system.LowestFloorServiced);

            system.GetElevator(elevatorId).RequestFloor(floorNum, Direction.None);
        }

        private static void HandleOutsideElevator(ElevatorSystem system, Building building)
        {
            string response = null;

            Console.WriteLine("What floor are you on?");
            int floorNum = 0;

            do
            {
                Console.WriteLine($"[{system.LowestFloorServiced} - {system.HighestFloorServiced}] Floor Number:> ");
                response = Console.ReadLine();

                if (CheckForExitCommand(response))
                {
                    return;
                }

            } while ((!int.TryParse(response, out floorNum)) || floorNum > system.HighestFloorServiced || floorNum < system.LowestFloorServiced);

            Floor floor = building.GetFloor(floorNum);

            response = PromptForInput("What direction do you want to go?", new string[] { "up", "down", "exit" });

            if (CheckForExitCommand(response))
            {
                return;
            }

            string capitalized = string.Concat(char.ToUpper(response[0]), response.Substring(1));
            floor.RequestElevator((Direction)Enum.Parse(typeof(Direction), capitalized));
        }

        private static string PromptForInput(string promptMessage, string[] expectedValues)
        {
            string response = null;

            do
            {
                Console.WriteLine(promptMessage);
                Console.WriteLine($"[{string.Join("/", expectedValues)}]:> ");
                response = Console.ReadLine().ToLowerInvariant();
            } while (!expectedValues.Where(v => string.Compare(v.ToLowerInvariant(), response) == 0).Any());

            return response;
        }
    }
}