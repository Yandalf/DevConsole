using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	static public class DevConsoleCommandSearcher
	{
		static public readonly string MacroString = "Name:{0},Types:{1}";


		static public void FindAllConsoleCommands(out IEnumerable<MethodInfo> methods, out IEnumerable<MethodInfo> macros, out IEnumerable<Type> types)
		{
			var allTypes	= AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
			var allMethods	= allTypes.SelectMany(t => t.GetMethods());
			methods = allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMethodAttribute)) > 0); //Finds all methods in the project marked with ConsoleMethodAttribute
			macros	= allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMacroAttribute)) > 0); //Finds all methods in the project marked with ConsoleMacroAttribute
			types	= allTypes.Where(t => t.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleClassAttribute)) > 0); //Finds all classes in the project marked with ConsoleClassAttribute.
		}

		static public string ConsoleCommandsToString(IEnumerable<MethodInfo> macros, Formatting formatting = Formatting.None)
        {
			var allCommands = new SerializableConsoleCommands(macros);
			return JsonConvert.SerializeObject(allCommands, formatting);
        }

		static public SerializableConsoleCommands StringToConsoleCommands(string commandsString)
        {
			return JsonConvert.DeserializeObject<SerializableConsoleCommands>(commandsString);
        }
	}

	public class SerializableConsoleCommands
    {
		public SerializableConsoleMacro[] consoleMacros;

		[JsonConstructor]
		public SerializableConsoleCommands() { }
		public SerializableConsoleCommands(IEnumerable<MethodInfo> macros)
        {
			consoleMacros = macros.Select(m => new SerializableConsoleMacro(m)).ToArray();
        }
    }

	public sealed class SerializableConsoleMacro
	{
		public ConsoleMacroAttribute attribute;
		public string assemblyName;
		public string typeName;
		public string methodName;


		[JsonConstructor]
		public SerializableConsoleMacro(ConsoleMacroAttribute attribute, string assemblyName, string typeName, string methodName)
		{
			this.attribute		= attribute;
			this.assemblyName	= assemblyName;
			this.typeName		= typeName;
			this.methodName		= methodName;
		}

		public SerializableConsoleMacro(ConsoleMacroAttribute attribute, MethodInfo methodInfo) : this(attribute, methodInfo.DeclaringType.Assembly.FullName, methodInfo.DeclaringType.FullName, methodInfo.Name)
		{ }

		public SerializableConsoleMacro(MethodInfo methodInfo) : this(methodInfo.GetCustomAttribute<ConsoleMacroAttribute>(), methodInfo)
		{ }

		static public explicit operator MethodInfo(SerializableConsoleMacro macro)
		{
			return Assembly.Load(macro.assemblyName).GetType(macro.typeName).GetMethod(macro.methodName);
		}
	}
}
