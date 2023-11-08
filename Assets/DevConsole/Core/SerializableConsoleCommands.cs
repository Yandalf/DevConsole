using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	public class SerializableConsoleCommands
    {
		public SerializableConsoleMethod[] consoleMethods;
		public SerializableConsoleMethod[] consoleMacros;

		[JsonConstructor]
		public SerializableConsoleCommands() { }
		public SerializableConsoleCommands(IEnumerable<MethodInfo> methods, IEnumerable<MethodInfo> macros)
        {
			consoleMethods = methods.Select(m => (SerializableConsoleMethod)m).ToArray();
			consoleMacros  = macros.Select(m => (SerializableConsoleMethod)m).ToArray();
        }

		public override string ToString()
		{
			return base.ToString() +
				$"ConsoleMethods:\n{string.Join("\n", consoleMethods.Select(m => $"-{m}"))}" +
				$"\nConsoleMacros:\n{string.Join("\n", consoleMacros.Select(m => $"-{m}"))}";
		}
	}
}
