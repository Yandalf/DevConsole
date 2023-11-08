using System;

namespace com.SolePilgrim.DevConsole
{
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
