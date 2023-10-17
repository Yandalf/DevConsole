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

	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleMethodAttribute : Attribute
	{
	}
}
