using Elevator;
using log4net;
using log4net.Config;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ElevatorSimulator
{
    class Program
    {
        private struct Options
        {
            public bool SimplePrompt { get; set; }
            public string[] Inputs { get; set; }
        }

        private const string EXIT_COMMAND = "exit";
        private static readonly Regex REQUEST_INPUT_FORMAT_REGEX = new Regex("^\\d+[DU]?$");
        private static ILog _log;
        private static Options _options;
        private static ElevatorSystem _system;

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            _log = LogManager.GetLogger(typeof(ElevatorSystem));

            _options = ParseArguments(args);

            _system = new ElevatorSystem(new IElevator[]{
                ElevatorFactory.Instance.Create(),
                ElevatorFactory.Instance.Create(),
            });

            Building.Initialize(_system);

            _system.RunAll();

            if (_options.Inputs.Any())
            {
                HandleRequestInputs(_options.Inputs);
            }

            if (_options.SimplePrompt)
            {
                DoSimpleUserPrompt();
            }
            else
            {
                DoInteractiveUserPrompt();
            }

            _system.StopAll();
        }

        private static void DoSimpleUserPrompt()
        {
            Thread userInputThread = new Thread(() =>
            {
                Console.WriteLine("Welcome to the Elevator System! " + Environment.NewLine +
                    "Please enter floor requests with optional direction in the format [RequestFloorNumber[Direction (either \"U\" or \"D\")]]. e.g. \"5D\", \"3\", or \"10U\".");
                try
                {
                    while (true)
                    {
                        Console.Write("::> ");
                        string response = Console.ReadLine().ToUpper();

                        if (CheckForExitCommand(response.ToLower()))
                        {
                            break;
                        }

                        if (!REQUEST_INPUT_FORMAT_REGEX.IsMatch(response))
                        {
                            Console.WriteLine($"Couldn't understand \"{response}\". Please enter inputs in the format \"8U\", \"17\", or \"9D\".");
                            continue;
                        }

                        string floorStr = response;
                        Direction requestedDir = Direction.None;

                        bool hasDirection = new Regex("[DU]$").IsMatch(response);

                        if (hasDirection)
                        {
                            floorStr = response.Substring(0, response.Length - 1);
                            string directionSelected = response.Substring(response.Length - 1);
                            requestedDir = Direction.Up;

                            if (string.CompareOrdinal(directionSelected, "D") == 0)
                            {
                                requestedDir = Direction.Down;
                            }
                        }

                        int floor;
                        if (!int.TryParse(floorStr, out floor))
                        {
                            Console.WriteLine($"Couldn't understand floor number {floorStr}.");
                            continue;
                        }

                        if (floor < _system.LowestFloorServiced.Number || floor > _system.HighestFloorServiced.Number)
                        {
                            Console.WriteLine($"Floor number {floor} is outside of the allowed range of {_system.LowestFloorServiced.Number} - {_system.HighestFloorServiced.Number}." +
                                " Please enter a floor number within that range.");
                            continue;
                        }

                        _system.RequestElevator(new FloorRequest(new Floor(floor), requestedDir));
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e.Message);
                    _log.Fatal(e.ToString());
                }
            })
            {
                IsBackground = false,
            };

            userInputThread.Start();

            userInputThread.Join();
        }

        private static void DoInteractiveUserPrompt()
        {
            Thread userInputThread = new Thread(() =>
            {
                Console.WriteLine("Welcome to the Elevator System! " + Environment.NewLine +
                    "Please enter selections at the prompts to enter floor requests. " + Environment.NewLine +
                    "Type \"exit\" at the prompt to terminate the program.");

                try
                {
                    while (true)
                    {
                        string response = PromptForInput("Are you already in the elevator?", new string[] { "y", "n", "exit" });

                        if (CheckForExitCommand(response))
                        {
                            break;
                        }
                        else if (response.CompareTo("n") == 0)
                        {
                            HandleOutsideElevator();
                        }
                        else
                        {
                            HandleInsideElevator();
                        }

                        Thread.Sleep(500);
                    }
                }
                catch (Exception e)
                {
                    _log.Error(e.Message);
                    _log.Fatal(e.ToString());
                }
            })
            {
                IsBackground = false,
            };

            userInputThread.Start();

            userInputThread.Join();
        }

        private static bool CheckForExitCommand(string command)
        {
            bool shouldExit = command.CompareTo(EXIT_COMMAND) == 0;

            if (shouldExit)
            {
                _log.Info("Received exit command!");
            }

            return shouldExit;
        }

        private static void HandleInsideElevator()
        {
            string response = PromptForInput("Which elevator are you in?", _system.Elevators.Select(i => i.ToString()).Concat(new string[] { "exit" }).ToArray());

            if (CheckForExitCommand(response))
            {
                return;
            }

            int elevatorId = int.Parse(response);

            Console.WriteLine("What floor do you want to go to?");
            int floorNum = 0;

            do
            {
                Console.WriteLine($"[{_system.LowestFloorServiced} - {_system.HighestFloorServiced}] Floor Number:> ");
                response = Console.ReadLine();
                if (CheckForExitCommand(response))
                {
                    return;
                }
            } while (!int.TryParse(response, out floorNum) && floorNum <= _system.HighestFloorServiced.Number && floorNum >= _system.LowestFloorServiced.Number);

            _system.GetElevator(elevatorId).RequestFloor(floorNum, Direction.None);
        }

        private static void HandleOutsideElevator()
        {
            string response = null;

            Console.WriteLine("What floor are you on?");
            int floorNum = 0;

            do
            {
                Console.WriteLine($"[{_system.LowestFloorServiced} - {_system.HighestFloorServiced}] Floor Number:> ");
                response = Console.ReadLine();

                if (CheckForExitCommand(response))
                {
                    return;
                }

            } while ((!int.TryParse(response, out floorNum)) || floorNum > _system.HighestFloorServiced.Number || floorNum < _system.LowestFloorServiced.Number);

            Floor floor = Building.Instance.GetFloor(floorNum);

            response = PromptForInput("What direction do you want to go?", new string[] { "up", "down", "exit" });

            if (CheckForExitCommand(response))
            {
                return;
            }

            string capitalized = string.Concat(char.ToUpper(response[0]), response.Substring(1));

            floor.SummonElevator(_system, (Direction)Enum.Parse(typeof(Direction), capitalized));
        }

        private static void HandleRequestInputs(string[] inputs)
        {
            foreach (string input in inputs)
            {
                if (!REQUEST_INPUT_FORMAT_REGEX.IsMatch(input))
                {
                    _log.Debug($"Couldn't understand \"{input}\". Please enter inputs in the format \"8U\", \"17\", or \"9D\".");

                    continue;
                }

                string floorStr = input;
                Direction requestedDir = Direction.None;

                bool hasDirection = new Regex("[DU]$").IsMatch(input);

                if (hasDirection)
                {
                    floorStr = input.Substring(0, input.Length - 1);
                    string directionSelected = input.Substring(input.Length - 1);
                    requestedDir = Direction.Up;

                    if (string.CompareOrdinal(directionSelected, "D") == 0)
                    {
                        requestedDir = Direction.Down;
                    }
                }

                int floor;
                if (!int.TryParse(floorStr, out floor))
                {
                    _log.Debug($"Couldn't understand floor number {floorStr}.");

                    continue;
                }

                if (floor < _system.LowestFloorServiced.Number || floor > _system.HighestFloorServiced.Number)
                {
                    _log.Debug($"Floor number {floor} is outside of the allowed range of {_system.LowestFloorServiced.Number} - {_system.HighestFloorServiced.Number}." +
                        " Please enter a floor number within that range.");

                    continue;
                }

                _system.RequestElevator(new FloorRequest(new Floor(floor), requestedDir));
            }
        }

        private static Options ParseArguments(string[] args)
        {
            if (args.Contains("/?") || args.Contains("?"))
            {
                ShowUsage();
                return new Options()
                {
                    SimplePrompt = false,
                    Inputs = null,
                };
            }

            var opts = new Options()
            {
                SimplePrompt = false,
                Inputs = null,
            };

            if (args.Contains("/s"))
            {
                opts.SimplePrompt = true;
            }

            if (args.Contains("/i"))
            {
                int index = Array.FindIndex(args, e => string.CompareOrdinal(e, "/i") == 0);
                if (args.Length > index + 1)
                {
                    opts.Inputs = args[index + 1].Split(',');
                }
            }

            return opts;
        }

        private static string PromptForInput(string promptMessage, string[] expectedValues)
        {
            string response = null;

            do
            {
                Console.WriteLine(promptMessage);
                Console.Write($"[{string.Join("/", expectedValues)}]:> ");
                response = Console.ReadLine().ToLowerInvariant();
            } while (!expectedValues.Where(v => string.Compare(v.ToLowerInvariant(), response) == 0).Any());

            return response;
        }

        private static void ShowUsage()
        {
            StringBuilder sb = new StringBuilder(":: Elevator System Simulator ::");
            sb.AppendLine("Options: [/s] [/i \"INPUTS, separated, by, commas\"] [/?]");
            sb.AppendLine("/s \t Simple input mode. Only one standard prompt will be presented. Inputs should follow INPUT format.");
            sb.AppendLine("/i \t Initial system inputs to be added before user inputs. Useful for setting initial state. Inputs will be added sequentially and must follow INPUT format");
            sb.AppendLine("/? \t Display this message.");
            sb.AppendLine();
            sb.AppendLine("INPUT formatting: Inputs should be strings taking the format of RequestFloorNumber[OptionalDirectionLetter (either \"U\" or \"D\")].\n" +
                "\t\"4U\" would be a request at floor 4 to go Up.\n" +
                "\t\"2\" would be a directionless request to stop at floor 2.\n" +
                "\t\"28D\" would be a request at floor 28 to go Down. Requests for floors out of the range of the system will be ignored.");

            Console.WriteLine(sb.ToString());
        }
    }
}