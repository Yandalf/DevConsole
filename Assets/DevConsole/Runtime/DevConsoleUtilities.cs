using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Contains methods for the Dev Console.</summary>
	static public class DevConsoleUtilities
	{
		//This Regex works as follows:
		//first symbol has to be underscore or lowercase letter, followed by any lowercase letters, numbers, or underscores (\w). This is group 1.
		//Next a single opening bracket, and at the very end a closing bracket. Between the brackets is group 2.
		//Group 2 accepts EITHER any amount of \w (0 or 1 argument), OR at least one \w followed by any repetitions of single "," followed by at least one \w (multiple arguments). This prevents empty arguments.
		/// <summary>Regex pattern for C# methods.</summary>
		static public readonly Regex CSharpMethodRegexPattern = new Regex("^([a-z_]+\\w*)\\((\\w*|\\w+(\\,\\w+)*)\\)$");
		//This has been added as Unity InstanceIDs are integers. May be useful in other applications, too.
		/// <summary>Regex pattern for integer number (positive and negative).</summary>
		static public readonly Regex IntegerRegexPattern = new Regex("^[\\d-]\\d*$");


		[ConsoleMethod]
		static public MethodInfo[] GetValidMethods(Type type)
		{
			//Must return all Methods that can be called on an object of Type type.
			throw new NotImplementedException();
		}
	}
}
