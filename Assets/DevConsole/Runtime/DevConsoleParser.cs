using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Holds Regexes for a DevConsole to parse input text with.</summary>
	public class DevConsoleParser
	{
		public readonly Regex methodRegex, typeRegex, nameRegex;
		public readonly char argumentSeparator;


		public DevConsoleParser(Regex methodRegex, Regex typeRegex, Regex nameRegex, char argumentSeparator)
		{
			this.methodRegex		= methodRegex;
			this.typeRegex			= typeRegex;
			this.nameRegex			= nameRegex;
			this.argumentSeparator	= argumentSeparator;
		}

		public string[] SplitArguments(string argumentsString)
		{
			return argumentsString.Split(argumentSeparator);
		}

		public bool IsName(string argument)
		{
			return nameRegex.IsMatch(argument);
		}

		public bool ParseFloat(string argument, out float result)
		{
			return float.TryParse(argument, NumberStyles.Float, CultureInfo.CreateSpecificCulture("en-us").NumberFormat, out result);
		}

		public bool ParseType(string argument, bool ignoreCase, out Type type)
		{
			type = Type.GetType(typeRegex.Match(argument)?.Groups["type"].Value, false, ignoreCase);
			return type != null;
		}

		public void GetMethodAndArguments(string command, out string methodName, out string[] arguments)
		{
			var match	= methodRegex.Match(command);
			methodName	= match.Groups["method"].Value;
			arguments	= SplitArguments(match.Groups["arguments"].Value);
		}
	}
}