using com.SolePilgrim.Instancing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	public delegate string ConsoleCommand(object commandTarget = null);

	public class DevConsole
	{
		public event EventHandler<Exception> OnException;

		public List<DevConsoleEntry> Entries { get; private set; } = new List<DevConsoleEntry>();
		public SerializableConsoleCommands Commands { get; private set; }
		public DevConsoleParser Parser { get; private set; }
		public InstanceMapper Mapper { get; private set; }


		public DevConsole(string serializedCommands, DevConsoleParser parser, InstanceMapper mapper)
		{
			Parser		= parser;
			Commands	= DevConsoleCommandSearcher.StringToConsoleCommands(serializedCommands);
			Mapper		= mapper;
		}

		public void EnterCommand(string input)
		{
			string output;
			try
			{
				output = ParseCommand(input).Invoke();
			}
			catch (Exception e)
			{
				output = e.Message + (e is CommandException ? string.Empty : $"\n{e.StackTrace}");
				OnException?.Invoke(this, e);
			}
			Entries.Add(new DevConsoleEntry(input, output));
		}

		public void ClearEntries()
		{
			Entries.Clear();
		}

		private ConsoleCommand ParseCommand(string command)
        {
			if (Mapper.IsInstanceID(command, out string instanceID))
            {
				return (object o) => 
				{
					var result = Mapper.GetObjectByInstanceID(instanceID);
					var s = $"{instanceID}-{result}";
					return s;
				};
            }
			else if (Parser.methodRegex.IsMatch(command))
			{
				Parser.GetMethodAndArguments(command, out string methodName, out string[] arguments);
				var methods	= Commands.consoleMethods.Where(m => string.Equals(m.methodName, methodName, StringComparison.OrdinalIgnoreCase)); //Find all methods with the correct name
				if (methods == null || methods.Count() == 0)
				{
					throw new BadParseCommandException(command);
				}
				var converted	= new object[0];
				var method		= methods.FirstOrDefault(c => ArgumentsMatch(arguments, c.parameterTypes, out converted)); //Find the method overload
				UnityEngine.Debug.Log($"Parsed Method: {methodName} Arguments: {string.Join(",",arguments)} Method: {method?.ToString() ?? "NULL"}");
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
				else
				{
					throw new BadArgumentCommandException(string.Join("\n",methods.Select(m => $"-{m.ToPrettyString()}")));
				}
			}
			throw new BadParseCommandException(command);
        }

		//TODO ensure mismatches between arguments and argumentTypes with auto-injections like DevConsole
		//Empty arguments now get mapped to DevConsole if no other overloads work either: dosomething() when only dosomething(devconsole) exists returns a match. This needs to be fixed.
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
					if (Parser.IsName(arguments[i]))
					{
						converted[i] = arguments[i];
					}
					else
					{
						return false;
					}
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
					if (Parser.ParseFloat(arguments[i], out float result))
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
					if (Mapper.GetObjectByInstanceID(arguments[i], out object result))
					{
						converted[i] = result;
					}
					else
					{
						return false;
					}
					UnityEngine.Debug.Log($"Mapped argument {arguments[i]} to InstanceID! Result: {result?.ToString()??"NULL"}");
				}
				if (argumentTypes[i] == typeof(Type))
				{
					if (Parser.ParseType(arguments[i], true, out var t))
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
		public BadArgumentCommandException(string commands) : base($"Invalid Argument(s) for DevConsoleCommand:\n{commands}") { }
	}
}