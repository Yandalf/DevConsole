using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Serialized representation of a MethodInfo object.</summary>
	public sealed class SerializableConsoleMethod
	{
		public readonly string assemblyName;
		public readonly string typeName;
		public readonly string methodName;
		public readonly string[] parameterTypeNames;
		[JsonIgnore]
		public readonly Type[] parameterTypes;


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

		/// <summary>Optional string notation of the ConsoleMethod.</summary>
		/// <param name="prettyPrint">If true, returns a minimal data notation. Else, returns ToString().</param>
		public string ToString(bool prettyPrint)
		{
			if (prettyPrint)
			{
				return $"{methodName}({string.Join(',', parameterTypes.Select(t => t.Name))})";
			}
			else
			{
				return ToString();
			}
		}

		public override string ToString()
		{
			return $"{nameof(SerializableConsoleMethod)}:{typeName}.{methodName}({string.Join(',', parameterTypeNames)})";
		}

		static public explicit operator MethodInfo(SerializableConsoleMethod method)
		{
			return Assembly.Load(method.assemblyName).GetType(method.typeName).GetMethod(method.methodName, method.parameterTypes);
		}

		static public explicit operator SerializableConsoleMethod(MethodInfo method)
		{
			return new SerializableConsoleMethod(method);
		}
	}
}
