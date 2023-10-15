using System.Text.RegularExpressions;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Holds Regexes for a DevConsole to parse input text with.</summary>
	public class DevConsoleParser
	{
		public readonly Regex instanceIDRegex, methodRegex;


		public DevConsoleParser(Regex instanceIdRegex, Regex methodRegex)
		{
			instanceIDRegex		= instanceIdRegex;
			this.methodRegex	= methodRegex;
		}
	}
}