using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	static public class DevConsoleCommandSearcher
	{
		static public void FindAllConsoleCommands(out IEnumerable<MethodInfo> methods, out IEnumerable<MethodInfo> macros)
		{
			var allTypes	= AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
			var allMethods	= allTypes.SelectMany(t => t.GetMethods());
			methods = allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMethodAttribute)) > 0); //Finds all methods in the project marked with ConsoleMethodAttribute
			macros	= allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMacroAttribute)) > 0); //Finds all methods in the project marked with ConsoleMacroAttribute
		}

		static public string ConsoleCommandsToString(IEnumerable<MethodInfo> methods, IEnumerable<MethodInfo> macros, Formatting formatting = Formatting.None)
        {
			var allCommands = new SerializableConsoleCommands(methods, macros);
			return JsonConvert.SerializeObject(allCommands, formatting);
        }

		static public SerializableConsoleCommands StringToConsoleCommands(string commandsString)
        {
			return JsonConvert.DeserializeObject<SerializableConsoleCommands>(commandsString);
        }
	}

	public class SerializableConsoleCommands
    {
		public SerializableConsoleMethod[] consoleMethods;
		public SerializableConsoleMethod[] consoleMacros;

		[JsonConstructor]
		public SerializableConsoleCommands() { }
		public SerializableConsoleCommands(IEnumerable<MethodInfo> methods, IEnumerable<MethodInfo> macros)
        {
			consoleMethods = methods.Select(m => new SerializableConsoleMethod(m)).ToArray();
			consoleMacros  = macros.Select(m => new SerializableConsoleMethod(m)).ToArray();
        }

		public override string ToString()
		{
			return base.ToString() +
				$"ConsoleMethods:\n{string.Join("\n", consoleMethods.Select(m => $"-{m.ToString()}"))}" +
				$"\nConsoleMacros:\n{string.Join("\n", consoleMacros.Select(m => $"-{m.ToString()}"))}";
		}
	}

	public sealed class SerializableConsoleMethod
	{
		public string assemblyName;
		public string typeName;
		public string methodName;
		public string[] parameterTypeNames;
		[JsonIgnore]
		public Type[] parameterTypes;


		[JsonConstructor]
		public SerializableConsoleMethod(string assemblyName, string typeName, string methodName, string[] parameterTypeNames)
		{
			this.assemblyName		= assemblyName;
			this.typeName			= typeName;
			this.methodName			= methodName;
			this.parameterTypeNames	= parameterTypeNames;
			parameterTypes			= parameterTypeNames.Select(t => Type.GetType(t)).ToArray();
		}

		public SerializableConsoleMethod(MethodInfo methodInfo) : 
			this(methodInfo.DeclaringType.Assembly.FullName, methodInfo.DeclaringType.FullName, methodInfo.Name, methodInfo.GetParameters().Select(p => p.ParameterType.AssemblyQualifiedName).ToArray())
		{ }

		public string ToPrettyString()
		{
			return $"{methodName}({string.Join(',', parameterTypes.Select(t => t.Name))})";
		}

		public override string ToString()
		{
			return $"{nameof(SerializableConsoleMethod)}:{typeName}.{methodName}({string.Join(',', parameterTypeNames)})";
		}

		static public explicit operator MethodInfo(SerializableConsoleMethod method)
		{
			return Assembly.Load(method.assemblyName).GetType(method.typeName).GetMethod(method.methodName, method.parameterTypes);
		}
	}
}
