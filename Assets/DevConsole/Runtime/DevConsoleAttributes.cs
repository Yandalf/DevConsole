using System;

namespace com.SolePilgrim.DevConsole
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field)]
	public class ConsoleVariableAttribute : Attribute
    {
		public string VariableName { get; private set; }


		public ConsoleVariableAttribute(string variableName)
        {
			VariableName = variableName;
        }
    }

	/// <summary>Attribute to mark static methods as callable through DevConsole. 
	/// Marked method can have arguments of types int, float, string, Type, and InstanceType of the Console's InstanceMapper.</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleMethodAttribute : Attribute {}

	/// <summary>Attrribute to mark static methods as callable through DevConsole.
	/// Marked method must have a single argument of type DevConsole for the console to inject itself in.</summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleMacroAttribute : Attribute {}
}
