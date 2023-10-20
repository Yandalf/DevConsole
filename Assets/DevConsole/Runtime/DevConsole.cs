using com.SolePilgrim.Instancing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

//TODO add a way for commands to differentiate between a string and a typename. Proposal: "string" is a string, "t:string" is a typename.
namespace com.SolePilgrim.DevConsole
{
	public delegate string ConsoleCommand(object commandTarget = null);

	public class DevConsole
	{
		public List<DevConsoleEntry> Entries { get; private set; } = new List<DevConsoleEntry>();
		public SerializableConsoleCommands Commands { get; private set; }
		public DevConsoleParser Parser { get; private set; }
		public InstanceMapper Mapper { get; private set; }

		static private readonly string InvalidCommandOutput = "Unrecognized Command!";


		public DevConsole(string serializedCommands, DevConsoleParser parser, InstanceMapper mapper)
		{
			Parser		= parser;
			Commands	= DevConsoleCommandSearcher.StringToConsoleCommands(serializedCommands);
			Mapper		= mapper;
		}

		public void EnterCommand(string input)
		{
			var output = ProcessInput(input);
			Entries.Add(new DevConsoleEntry(input, output));
		}

		public void ClearEntries()
		{
			Entries.Clear();
		}

		private string ProcessInput(string input)
		{
            try
            {
				return ParseCommand(input).Invoke();
			}
			catch(Exception e)
            {
				return e.Message + (e is CommandException ? string.Empty : $"\n{e.StackTrace}");
			}
		}

		private ConsoleCommand ParseCommand(string command)
        {
			if (Parser.instanceIDRegex.IsMatch(command))
            {
				return (object o) => 
				{
					var result = Mapper.GetObjectByInstanceID(command);
					var s = $"{command}-{result}";
					return s;
				};
            }
			else if (Parser.methodRegex.IsMatch(command))
			{
				var converted		= new object[0];
				var match			= Parser.methodRegex.Match(command);
				var methodGroup		= match.Groups.FirstOrDefault(g => g.Name == "method");
				var arguments		= Parser.SplitArguments(match.Groups.FirstOrDefault(g => g.Name == "arguments")?.Value);
				var method			= Commands.consoleMethods.Where(m => string.Equals(m.methodName, methodGroup.Value, StringComparison.OrdinalIgnoreCase)). //Find all methods with the correct name
					FirstOrDefault(c => ArgumentsMatch(arguments, c.parameterTypes, out converted)); //Find the method overload
				UnityEngine.Debug.Log($"Parsed Method: {methodGroup.Value} Arguments: {string.Join(",",arguments)} Method: {method?.ToString() ?? "NULL"}");
				if (method != null)
				{
					return (object o) =>
					{
						var info = (MethodInfo)method;
						if (info.ReturnType == typeof(string))
						{
							return (string)info.Invoke(null, converted);
						}
						info.Invoke(null, converted);
						return $"Executed {method.methodName}";
					};
				}
				else //TODO differentiate between unrecognized command and bad arguments. "Foo1" should throw unrecognized command, "Foo()" should throw bad arguments
				{
					throw new BadArgumentCommandException(command);
				}
			}
			throw new BadParseCommandException(command);
        }

		//TODO ensure mismatches between arguments and argumentTypes with auto-injections like DevConsole
		private bool ArgumentsMatch(string[] arguments, Type[] argumentTypes, out object[] converted)
		{
			UnityEngine.Debug.Log($"Matching Arguments: ({(arguments == null ? "NONE" : string.Join(",", arguments))}) " +
				$"to Types: ({(argumentTypes == null ? "NONE" : string.Join(",",argumentTypes.Select(t => t?.Name ?? "NOTYPE")))})");
			converted = new object[argumentTypes.Length];
			if (arguments.Length != argumentTypes.Length)
			{
				UnityEngine.Debug.Log($"Arguments {arguments.Length} and Types {argumentTypes.Length} Length Mismatch!");
				return false;
			}

			for (int i = 0; i < arguments.Length && i < argumentTypes.Length; i++)
			{
				if (argumentTypes[i] == typeof(string))
				{
					converted[i] = arguments[i];
				}
				else if (argumentTypes[i] == typeof(int))
				{
					if (int.TryParse(arguments[i], out int result))
					{
						converted[i] = result;
					}
					else
					{
						return false;
					}
				}
				else if (argumentTypes[i] == typeof(float))
				{
					if (float.TryParse(arguments[i], NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-us").NumberFormat, out float result))
					{
						converted[i] = result;
					}
					else
					{
						return false;
					}
				}
				else if (Mapper.InstanceType.IsAssignableFrom(argumentTypes[i]))
				{
					if (Mapper.IsInstanceID(arguments[i]))
					{
						var result = Mapper.GetObjectByInstanceID(arguments[i]);
						if (result == null)
						{
							return false;
						}
						converted[i] = result;
					}
					else
					{
						return false;
					}
				}
				else if (argumentTypes[i] == typeof(Type))
				{
					var t = Type.GetType(arguments[i], false, true);
					if (t != null)
					{
						converted[i] = t;
						UnityEngine.Debug.Log($"Mapped argument {arguments[i]} to Type!");
					}
					else
					{
						return false;
					}
				}
				else if (argumentTypes[i] == typeof(DevConsole))
				{
					converted[i] = this;
					UnityEngine.Debug.Log($"Mapped argument {arguments[i]} to DevConsole!");
				}
			}
			return true;
		}
	}

	abstract public class CommandException : Exception
	{
		public CommandException(string message) : base(message) { }
	}

	public class BadParseCommandException : CommandException
	{
		public BadParseCommandException(string command) : base($"Unrecognized DevConsoleCommand:{command}") { }
	}

	public class BadArgumentCommandException : CommandException
	{
		public BadArgumentCommandException(string command) : base($"Invalid DevConsoleCommand Arguments") { }
	}
}