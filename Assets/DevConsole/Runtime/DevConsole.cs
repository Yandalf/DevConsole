using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.SolePilgrim.DevConsole
{
    public class DevConsole
	{
		private delegate object ConsoleCommand(out string log, object commandTarget = null);

		public List<DevConsoleEntry> Entries { get; private set; } = new List<DevConsoleEntry>();
		public SerializableConsoleCommands Commands { get; private set; }

		private readonly Regex _instanceIdRegex;
		//This Regex works as follows:
		//first symbol has to be underscore or lowercase letter, followed by any lowercase letters, numbers, or underscores (\w). This is group 1.
		//Next a single opening bracket, and at the very end a closing bracket. Between the brackets is group 2.
		//Group 2 accepts EITHER any amount of \w (0 or 1 argument), OR at least one \w followed by any repetitions of single "," followed by at least one \w (multiple arguments). This prevents empty arguments.
		private readonly Regex _methodRegex = new("^([a-z_]+\\w*)\\((\\w*|\\w+(\\,\\w+)*)\\)$");

		static private readonly string InvalidCommandOutput = "Unrecognized Command!";


		public DevConsole(string serializedCommands, string instanceIdRegexPattern)
        {
			Commands = DevConsoleCommandSearcher.StringToConsoleCommands(serializedCommands);
			_instanceIdRegex = new Regex(instanceIdRegexPattern);
        }

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
            try
            {
				//Lowercase everything and remove unnecessary spaces
				var discreteCommands	= input.ToLower().Replace(" ","").Split('.').Select(t => ParseCommand(t)).ToArray();
				var log					= string.Empty;
				var commandReturn		= discreteCommands[0].Invoke(out log);
				for (int i = 1; i < discreteCommands.Length; i++)
				{
					commandReturn = discreteCommands[i].Invoke(out string logAdd, commandReturn);
					log += "\n" + logAdd;
				}
				return log;
			}
			catch(Exception e)
            {
				return $"{e.Message}\n{e.StackTrace}";
            }
		}

		private ConsoleCommand ParseCommand(string command)
        {
			UnityEngine.Debug.Log(command);
			if (_instanceIdRegex.IsMatch(command))
            {
				UnityEngine.Debug.Log($"Command {command} is an InstanceID!");
            }
			else if (_methodRegex.IsMatch(command))
			{
				UnityEngine.Debug.Log($"Command {command} is a method!");
			}
			return null;
        }
	}
}