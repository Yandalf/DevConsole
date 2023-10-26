# DevConsole
DevConsole is an extensible console to debug and manipulate a game at runtime.
It is primarily inspired by the command console seen in Bethesda Game Studios' games (Fallout, The Elder Scrolls).

## Requirements
* .Net

## Getting Started
### Creating a Console
A DevConsole object must be instantiated with a string of serialized commands, a DevConsoleParser, one or more InstanceMappers, and one or more DevConsoleArgumentMatchers.
DevConsoleParser is responsible for interpreting string input to searchable name and arguments within the given serialized commands.
Instances mapped through InstanceMappers are searchable by the console. A single mapper mapping objects is sufficient, but more specific implementations can offer more granularity.
DevConsoleArgumentMatchers convert string arguments to concrete types to be used when the command is invoked.

DevConsoleParser uses Regexes to recognize methods, types, and name inputs. Default regexes are listed in DevConsoleUtilities.
* DevConsoleUtilities.CSharpMethodRegex matches text in the format "methodname(arguments)", arguments can be empty. Multiple arguments can be split with a comma, and decimal numbers can have a period. Methodname is stored in the group "method" and arguments are stored in the group "arguments".
* DevConsoleUtilities.TypeRegex matches text in the format "t:namespace.typename" and stores namespace.typename in the "type" group.

In Unity a DevConsoleComponent exists with an example implementation.
DevConsoleComponent creates and manages a DevConsole and draws an IMGUI when toggled. Additionally DevConsoleComponent allows the user to click a GameObject to print its InstanceID.

### Creating Commands
DevConsole provides two attributes to mark methods in your project's codebase with: ConsoleMethodAttribute and ConsoleMacroAttribute.
These attributes mark methods for serialization as a Command to be used with a DevConsole.

ConsoleMethodAttribute must be used with static methods. Methods can have any number of arguments of types string, int, float, Type, and any types mapped by an InstanceMapper (see below).
ConsoleMacroAttribute must be used with static methods. These methods must have a single argument of type DevConsole.
If the marked method returns a string, the resulting string will be printed in the DevConsole whenever the method is called through it.

To serialize the Commands, find all commands with DevConsoleCommandSearcher.FindAllConsoleCommands, and serialize the output with DevConsoleCommandSearcher.ConsoleCommandsToString.
Deserialize to a SerializedCommands object with DevConsoleCommandSearcher.StringToConsoleCommands.

In Unity a menu is added at <b>SolePilgrim/DevConsole/Update Console Commands</b> to run the serialization and store the output in a TextAsset at <b>Resources/DevConsole/ConsoleCommands.txt</b>

### InstanceMappers
InstanceMappers map active objects by an InstanceID for easier lookup.
InstanceMappers can find mapped objects by either being given an InstanceID directly or a string that matches their InstanceIDRegex.

In Unity a UnityObjectInstanceMapperComponent exists with an example implementation.
UnityObjectInstanceMapperComponent creates and manages an InstanceMapper<int,GameObject> mapping a GameObject to its runtime InstanceID [(see Unity Documentation)](https://docs.unity3d.com/ScriptReference/Object.GetInstanceID.html). 
The InstanceIDRegex matches text in the format "i:instanceid" and stores the instanceid in the "id" group.

## Authors
* Yannick "Yandalf" Vanhoutte

## Version History
* 0.1
    * Initial Release
