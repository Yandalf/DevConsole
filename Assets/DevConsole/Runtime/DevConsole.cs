using com.SolePilgrim.Instancing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.SolePilgrim.DevConsole
{
	public delegate object ConsoleCommand(out string log, object commandTarget = null);

	public class DevConsole
	{
		public List<DevConsoleEntry> Entries { get; private set; } = new List<DevConsoleEntry>();
		public SerializableConsoleCommands Commands { get; private set; }
		public DevConsoleParser Parser { get; private set; }
		public InstanceMapper Mapper { get; private set; }

		static private readonly string InvalidCommandOutput = "Unrecognized Command!";


		public DevConsole(string serializedCommands, DevConsoleParser parser, InstanceMapper mapper)
		{
			Parser		= parser;
			Commands	= DevConsoleCommandSearcher.StringToConsoleCommands(serializedCommands);
			Mapper		= mapper;
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
			if (Parser.instanceIDRegex.IsMatch(command))
            {
				var instanceID = int.Parse(command);
				return (out string s, object o) => 
				{
					var result = Mapper.GetObjectByInstanceID(instanceID);
					s = $"{instanceID}-{result}";
					return result;
				};
            }
			else if (Parser.methodRegex.IsMatch(command))
			{
				var matches = Parser.methodRegex.Matches(command);
				UnityEngine.Debug.Log($"Command {command} is a method! Matches: {string.Join("|", matches.Select(m => string.Join(",", m.Groups)))}");
				var method = Commands.consoleMethods.First(m => m.methodName == matches.First().Groups.First().Value); //TODO working on this!
				return (out string s, object o) =>
				{
					s = $"";
					return o;
				};
			}
			return null;
        }
	}
}