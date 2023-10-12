using System.Text.RegularExpressions;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Holds Regexes for a DevConsole to parse input text with.</summary>
	public class DevConsoleParser
	{
		public readonly Regex instanceIDRegex, methodRegex;


		public DevConsoleParser(string instanceIdPattern, string methodPattern)
		{
			instanceIDRegex = new Regex(instanceIdPattern);
			methodRegex		= new Regex(methodPattern);
		}
	}
}