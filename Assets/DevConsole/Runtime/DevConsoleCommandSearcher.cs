using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	static public class DevConsoleCommandSearcher
	{
		static public void FindAllConsoleCommands(out IEnumerable<MethodInfo> methods)
		{
			var allTypes	= AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
			var allMethods	= allTypes.SelectMany(t => t.GetMethods());
			methods = allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMethodAttribute)) > 0); //Finds all methods in the project marked with ConsoleMethodAttribute
		}

		static public string ConsoleCommandsToString(IEnumerable<MethodInfo> methods, Formatting formatting = Formatting.None)
        {
			var allCommands = new SerializableConsoleCommands(methods);
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

		[JsonConstructor]
		public SerializableConsoleCommands() { }
		public SerializableConsoleCommands(IEnumerable<MethodInfo> methods)
        {
			consoleMethods = methods.Select(m => new SerializableConsoleMethod(m)).ToArray();
        }

		public override string ToString()
		{
			return base.ToString() + 
				$"ConsoleMethods:\n{string.Join("\n",consoleMethods.Select(m => $"-{m.ToString()}"))}";
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
