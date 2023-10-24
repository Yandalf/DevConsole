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

		public List<DevConsoleEntry> Entries				{ get; private set; } = new List<DevConsoleEntry>();
		public SerializableConsoleCommands Commands			{ get; private set; }
		public DevConsoleParser Parser						{ get; private set; }
		public InstanceMapper[] InstanceMappers				{ get; private set; }
		public DevConsoleArgumentMatcher[] ArgumentMatchers { get; private set; }


		public DevConsole(string serializedCommands, DevConsoleParser parser, InstanceMapper[] mappers, DevConsoleArgumentMatcher[] matchers)
		{
			Parser				= parser;
			Commands			= DevConsoleCommandSearcher.StringToConsoleCommands(serializedCommands);
			InstanceMappers		= mappers;
			ArgumentMatchers	= matchers;
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
			var instanceID		= string.Empty;
			var matchingMapper	= InstanceMappers.FirstOrDefault(m => m.IsInstanceID(command, out instanceID));
			if (matchingMapper != null)
            {
				return () => 
				{
					var result = matchingMapper.GetObjectByInstanceID(instanceID);
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
				for (int j = 0; j < ArgumentMatchers.Length; j++)
				{
					var matched = ArgumentMatchers[j].TryConvertArgument(arguments[i], argumentTypes[i], this, out converted[i]);
					if (matched)
					{
						break;
					}
					else if (j == ArgumentMatchers.Length - 1)
					{
						return false; //Return false as soon as we can't match a single argument
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