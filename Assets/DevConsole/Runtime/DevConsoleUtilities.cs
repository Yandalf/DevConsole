using System;
using System.Linq;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Contains methods for the Dev Console.</summary>
	static public class DevConsoleUtilities
	{
		//This Regex works as follows:
		//first symbol has to be underscore or lowercase letter, followed by any lowercase letters, numbers, or underscores (\w). This is group 1 named method.
		//Next a single opening bracket, and at the very end a closing bracket. Between the brackets is group 2 named arguments.
		//Group 2 accepts any amount of \w, commas, points, colons, and scores. Commas cannot lead or end and cannot be doubled, to prevent empty arguments.
		/// <summary>Regex pattern for C# methods.</summary>
		static public readonly string CSharpMethodRegex = @"^(?<method>[a-z][\w]*)\((?<arguments>([\w-.:]+(?:,)?)*)(?<!,)\)$";
		/// <summary>Regex pattern for integer number (positive or negative).</summary>
		static public readonly string IntegerRegex = @"(-?\d+)";
		/// <summary>Regex pattern for decimal number (positive or negative. Use "." to denote decimals).</summary>
		static public readonly string DecimalRegex = @"(-?\d*\.?\d*)";
		/// <summary>Regex pattern for variable and method names.</summary>
		static public readonly string NameRegex = @"(?<!t:)([a-z][\w]*)";
		/// <summary>Regex pattern for type arguments.</summary>
		static public readonly string TypeRegex = @"(t:(?<type>[a-z_][\w_.]*))";


		[ConsoleMacro]
		static public string LogInstances(DevConsole devConsole)
		{
			var result = string.Empty;
			foreach (var mapper in devConsole.InstanceMappers)
			{
				mapper.UpdateMapping();
				result += $"Current Instances of type {mapper.InstanceType.Name}:";
				var mappings = mapper.GetMappings();
				foreach (var kvp in mappings)
				{
					result += $"\n{kvp.Key.ToString()}-{kvp.Value.ToString()}";
				}
				if (devConsole.InstanceMappers.Last() != mapper)
				{
					result += "\n";
				}
			}
			return result;
		}

		[ConsoleMacro]
		static public string Help(DevConsole devConsole)
		{
			return $"Methods:\n{string.Join("\n", devConsole.Commands.consoleMethods.OrderBy(m => m.methodName).Select(m => $" -{m.ToPrettyString()}"))}" +
				$"\nMacros:\n{string.Join("\n", devConsole.Commands.consoleMacros.OrderBy(m => m.methodName).Select(m => $" -{m.ToPrettyString()}"))}";
		}

		[ConsoleMethod]
		static public string Foo()
		{
			return $"Foo()";
		}

		[ConsoleMethod]
		static public string Foo(int arg)
		{
			return $"Foo int:{arg}";
		}

		[ConsoleMethod]
		static public string Foo(float arg)
		{
			return $"Foo float:{arg}";
		}

		[ConsoleMethod]
		static public string Foo(string arg)
		{
			return $"Foo string:{arg}";
		}

		[ConsoleMethod]
		static public string Foo(Type type)
		{
			return $"Foo type:{type.Name}";
		}

		[ConsoleMethod]
		static public string Foo(string arg1, int arg2, float arg3)
		{
			return $"foo string int float:{arg1} {arg2} {arg3}";
		}
	}
}
