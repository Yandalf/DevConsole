using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.SolePilgrim.DevConsole
{
	static public class DevConsoleCommandSearcher
	{
		static public void FindAllConsoleCommands(out IEnumerable<MethodInfo> methods, out IEnumerable<MethodInfo> macros, out IEnumerable<Type> types)
		{
			var allTypes	= AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
			var allMethods	= allTypes.SelectMany(t => t.GetMethods());
			methods = allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMethodAttribute)) > 0); //Finds all methods in the project marked with ConsoleMethodAttribute
			macros	= allMethods.Where(m => m.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleMacroAttribute)) > 0); //Finds all methods in the project marked with ConsoleMacroAttribute
			types	= allTypes.Where(t => t.CustomAttributes.Count(d => d.AttributeType == typeof(ConsoleClassAttribute)) > 0); //Finds all classes in the project marked with ConsoleClassAttribute.
		}
	}
}
