# Elevator Simulator
This project simulates how a system of elevators might move between floors in a building based on the requests received from inside and outside the elevators in the system.
The simulator is currently designed to handle multiple elevators per building and designed to be extensible so that the concept of elevator banks serving only specific floors could be added.

As of July 28, 2017, the project simulates a 10 story building with 2 elevators that serve all floors.

## Technologies
This project was built against the dotNet Core runtime and should be runnable on any platform that can run the dotNet Core including [Windows](https://www.microsoft.com/net/core#windowscmd), [Linux](https://www.microsoft.com/net/core#linuxubuntu), & [MacOS 10.11+](https://www.microsoft.com/net/core#macos).

In order to run the program, you must install the [dotNet Core runtime](https://www.microsoft.com/net/core) and execute the following commands from the directory where the `ElevatorSimulator.csproj` file is stored:
```
dotnet restore

dotnet run
```
Command line arguments may be specified in the `launchsettings.json` file in the Properties directory.

This project relies on [log4net](https://logging.apache.org/log4net/index.html) for logging and [xUnit](https://xunit.github.io/) for unit testing.

## Requests
The elevator system can receive requests of 2 types. 

The first type of request is meant to model a request coming from outside the elevators when summoning an elevator to your floor to pick you up. 
This sort of request includes a desired direction of travel. For example, if you were on the 4th floor and you wanted to go to the 10th floor, you would press the UP button on the 4th floor and expect
the elevator to pick you up on its way up. 

The second type of request is meant to model a floor request made from inside the elevator once you have boarded a specific car. To use the previous example, once you boarded the elevator car on the 4th floor,
 you would then select the 10 button on the inside of the elevator indicating your desire to go to the 10th floor. The elevator should then move towards the 10th floor, continuing to service relevant requests
 along the way, and stop there to let you disembark.

These requests can be entered at the prompts that the program presents you with or passed in via command line arguments when launching the program.

## Usage
In interactive input mode you should be presented with prompts for input. If at any point you wish to exit the program, simply enter "exit" at the prompt. If status output obscures the prompt, simply enter an empty line to display it again.

```
Options: [/s] [/i "INPUTS,separated,by,commas"] [/?]
 /s    Simple input mode. Only one standard prompt will be presented. Inputs should follow INPUT format
 /i    Initial system inputs to be added before user inputs. Useful for setting initial state. Inputs will be added sequentially and must follow INPUT format
 /?    Display usage instructions

INPUT formatting: Inputs should be strings taking the format of RequestFloorNumber[OptionalDirectionLetter (either "U" or "D")]
  "4U" would be a request at floor 4 to go Up.
  "2" would be a directionless request to stop at floor 2.
  "28D" would be a request at floor 28 to go Down. Requests for floors out of the range of the system will be ignored.
```
Arguments may be passed to the program on launch by editing the `launchsettings.json` file in the Properties directory. 
Example `launchsettings.json` file which specifies simple input mode and a set of initial requests:
```json
{
  "profiles": {
    "ElevatorSimulator": {
      "commandName": "Project",
      "commandLineArgs": "/s /i \"1D,9,3U,55D,7U\""
    }
  }
}
```
