using System.Collections.Generic;

namespace com.SolePilgrim.DevConsole
{
	public class DevConsole
	{
		public List<DevConsoleEntry> Entries { get; private set; } = new List<DevConsoleEntry>();

		static private readonly string InvalidCommandOutput = "Unrecognized Command!";


		public void EnterCommand(string input)
		{
			var output = ProcessInput(input);
			Entries.Add(new DevConsoleEntry(input, output));
		}

		public void ClearEntries()
		{
			Entries.Clear();
		}

		private string ProcessInput(string input)
		{
			var openBracketIndex	= input.IndexOf('(');
			var closeBracketIndex	= input.LastIndexOf(')');
			var commandParts	= input.Substring(0, openBracketIndex > 0 ? openBracketIndex : input.Length).Split('.');
			var commandArgs		= input.Substring(openBracketIndex + 1, closeBracketIndex > 0 ? closeBracketIndex - openBracketIndex - 1 : 0).Split(',');
			//TODO istead of naievely splitting Args, we should partition into discrete method groups. This to allow constructions like "object.method1(arg1, arg2).method2().method3(arg1)"
			//UnityEngine.Debug.Log($"CommandParts: {string.Join(',', commandParts)}\nArguments: {string.Join(',', commandArgs)}");
			//TODO
			//First check Parts to figure out if we have a Macro or a regular Command structure. 
			//-Macro: single part that corresponds to a Macro Name.
			//-Command: An Instance ID to be turned into an object followed by one or more valid commands.
			//Next check if Args neatly fit the given command or macro.
			return InvalidCommandOutput;
		}
	}

	/// <summary>DevConsole Command Input and Output data.</summary>
	public struct DevConsoleEntry
	{
		public string Input		{ get; private set; }
		public string Output	{ get; private set; }

		static private readonly int _paddingSpaces = 4;


		public DevConsoleEntry(string input, string output)
		{
			Input = input;
			Output = output;
		}

		public override string ToString()
		{
			return Input +
				(string.IsNullOrEmpty(Output) ? "" :
				"\n" + new string(' ', _paddingSpaces) + Output.Replace("\n", "\n" + new string(' ', _paddingSpaces)));
		}
	}
}