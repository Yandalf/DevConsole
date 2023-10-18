using System;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Contains methods for the Dev Console.</summary>
	static public class DevConsoleUtilities
	{
		//This Regex works as follows:
		//first symbol has to be underscore or lowercase letter, followed by any lowercase letters, numbers, or underscores (\w). This is group 1 named method.
		//Next a single opening bracket, and at the very end a closing bracket. Between the brackets is group 2 named arguments.
		//Group 2 accepts any amount of \w, commas, points, and scores. Commas cannot lead or end. This prevents empty arguments. //TODO prevent doubling!
		/// <summary>Regex pattern for C# methods.</summary>
		static public readonly string CSharpMethodRegex = "^(?<method>[a-z_]\\w*)\\((?<arguments>(?!,)[\\w,.-]*)(?<!,)\\)$";
		/// <summary>Regex pattern for integer number (positive and negative).</summary>
		static public readonly string IntegerRegex = "^(-?\\d+)$";
		/// <summary>Regex pattern for decimal number (positive or negative. Use "." to denote decimals).</summary>
		static public readonly string DecimalRegex = "^(-?\\d*\\.?\\d*)$";
		/// <summary>Regex pattern for variable and method names.</summary>
		static public readonly string NameRegex = "^([a-z_]\\w*)$";


		[ConsoleMethod]
		static public MethodInfo[] GetValidMethods(Type type)
		{
			//Must return all Methods that can be called on an object of Type type.
			throw new NotImplementedException();
		}

		[ConsoleMethod]
		static public void Foo(int arg)
		{

		}

		[ConsoleMethod]
		static public void Foo(float arg)
		{

		}

		[ConsoleMethod]
		static public void Foo(string arg)
		{

		}
	}
}
