using System;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Contains methods for the Dev Console.</summary>
	static public class DevConsoleUtilities
	{
		[ConsoleMethod]
		static public MethodInfo[] GetValidMethods(Type type)
		{
			//Must return all Methods that can be called on an object of Type type.
			throw new NotImplementedException();
		}
	}
}
