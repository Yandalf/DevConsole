using System;

namespace com.SolePilgrim.DevConsole
{
	[AttributeUsage(AttributeTargets.Method)]
	public class ConsoleMethodAttribute : Attribute
	{

	}

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
	public class ConsoleMacroAttribute : Attribute
	{
		public string MacroName { get; private set; }
		public Type[] ArgumentTypes { get; private set; }


		public ConsoleMacroAttribute(string macroName, Type[] args)
		{
			MacroName = macroName;
			ArgumentTypes = args;
		}
	}

	[AttributeUsage(AttributeTargets.Class)]
	public class ConsoleClassAttribute : Attribute
	{

	}
}
