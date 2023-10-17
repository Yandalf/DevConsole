using System.Text.RegularExpressions;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Holds Regexes for a DevConsole to parse input text with.</summary>
	public class DevConsoleParser
	{
		public readonly Regex instanceIDRegex, methodRegex;
		public readonly char argumentSeparator;


		public DevConsoleParser(Regex instanceIdRegex, Regex methodRegex, char argumentSeparator)
		{
			instanceIDRegex			= instanceIdRegex;
			this.methodRegex		= methodRegex;
			this.argumentSeparator	= argumentSeparator;
		}

		public string[] SplitArguments(string argumentsString)
		{
			return argumentsString.Split(argumentSeparator);
		}
	}
}