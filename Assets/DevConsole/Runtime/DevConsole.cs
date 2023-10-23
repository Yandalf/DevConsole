using com.SolePilgrim.Instancing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	public class DevConsole
	{
		public delegate string ConsoleCommand();
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

		//TODO Foo() throws exception in execution now! 
		private ConsoleCommand ParseCommand(string command)
        {
			if (Mapper.IsInstanceID(command, out string instanceID))
            {
				return () => 
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
				var macros	= Commands.consoleMacros.Where(m => string.Equals(m.methodName, methodName, StringComparison.OrdinalIgnoreCase));
				if (methods?.Count() == 0 && macros?.Count() == 0)
				{
					throw new BadParseCommandException(command);
				}
				var methodParams	= new object[0];
				var method			= methods.FirstOrDefault(c => ArgumentsMatch(arguments, c.parameterTypes, out methodParams)); //Find the method overload
				var macro			= macros.FirstOrDefault(c => c.parameterTypes.Length == 1 && c.parameterTypes.Contains(typeof(DevConsole))); //Find the macro that only has a single DevConsole argument
				if (method != null)
				{
					return Convert((MethodInfo)method, null, methodParams);
				}
				else if (macro != null)
				{
					return Convert((MethodInfo)macro, null, new object[] { this });
				}
				else
				{
					throw new BadArgumentCommandException(string.Join("\n",methods.Select(m => $"-{m.ToPrettyString()}").Concat(macros.Select(m => $"-{m.ToPrettyString()}"))));
				}
			}
			throw new BadParseCommandException(command);
        }

		private bool ArgumentsMatch(string[] arguments, Type[] argumentTypes, out object[] converted)
		{
			converted = new object[argumentTypes.Length];
			if (arguments.Length != argumentTypes.Length)
			{
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
				}
				else if (argumentTypes[i] == typeof(Type))
				{
					if (Parser.ParseType(arguments[i], true, out var t))
					{
						converted[i] = t;
					}
					else
					{
						return false;
					}
				}
			}
			return true;
		}

		private ConsoleCommand Convert(MethodInfo method, object caller, object[] parameters)
		{
			return () =>
			{
				if (method.ReturnType == typeof(string))
				{
					return (string)method.Invoke(caller, parameters);
				}
				method.Invoke(caller, parameters);
				return $"Executed {method.Name}";
			};
		}
	}
}