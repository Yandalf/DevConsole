using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Tools to generate and serialize ConsoleCommands with.</summary>
	static public class CommandSearcher
	{
		/// <summary>Searches the entire AppDomain for all methods marked for use in the console.</summary>
		/// <param name="methods">All methods marked with ConsoleMethodAttribute.</param>
		/// <param name="macros">All methods marked with ConsoleMacroAttribute.</param>
		static public void FindAllConsoleCommands(out IEnumerable<MethodInfo> methods, out IEnumerable<MethodInfo> macros)
		{
			var allTypes	= AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
			var allMethods	= allTypes.SelectMany(t => t.GetMethods());
			methods = allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMethodAttribute)) > 0); //Finds all methods in the project marked with ConsoleMethodAttribute
			macros	= allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMacroAttribute)) > 0); //Finds all methods in the project marked with ConsoleMacroAttribute
		}

		/// <summary>Serializes the given methods and macros to a json string formatted as SerializableConsoleCommands.</summary>
		static public string ConsoleCommandsToString(IEnumerable<MethodInfo> methods, IEnumerable<MethodInfo> macros, Formatting formatting = Formatting.None)
        {
			var allCommands = new SerializableConsoleCommands(methods, macros);
			return JsonConvert.SerializeObject(allCommands, formatting);
        }

		/// <summary>Deserializes a json string to a SerializableConsoleCommands object.</summary>
		static public SerializableConsoleCommands StringToConsoleCommands(string commandsString)
        {
			return JsonConvert.DeserializeObject<SerializableConsoleCommands>(commandsString);
        }
	}
}
