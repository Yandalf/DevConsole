using System;
using System.Linq;

namespace com.SolePilgrim.DevConsole
{
	/// <summary>Rules to convert a string argument.</summary>
	abstract public class DevConsoleArgumentMatcher
	{
		/// <summary>Attempts to convert a string argument.</summary>
		/// <param name="argument">String to convert.</param>
		/// <param name="argumentType">Type of argument to match.</param>
		/// <param name="devConsole">DevConsole providing additional context.</param>
		/// <param name="converted">Converted value, if successful.</param>
		/// <returns>True if successfully converted, else false.</returns>
		abstract public bool TryConvertArgument(string argument, Type argumentType, DevConsole devConsole, out object converted);
	}

	/// <summary>Converts arguments to int objects.</summary>
	public class IntMatcher : DevConsoleArgumentMatcher
	{
		public override bool TryConvertArgument(string argument, Type argumentType, DevConsole devConsole, out object converted)
		{
			if (argumentType != typeof(int))
			{
				converted = null;
				return false;
			}
			var success = int.TryParse(argument, out var result);
			converted	= result;
			return success;
		}
	}

	/// <summary>Converts arguments to float objects.</summary>
	public class FloatMatcher : DevConsoleArgumentMatcher
	{
		public override bool TryConvertArgument(string argument, Type argumentType, DevConsole devConsole, out object converted)
		{
			if (argumentType != typeof(float))
			{
				converted = null;
				return false;
			}
			var success = devConsole.Parser.ParseFloat(argument, out var result);
			converted	= result;
			return success;
		}
	}

	/// <summary>Converts arguments to string objects using DevConsole's Name Parser.</summary>
	public class NameMatcher : DevConsoleArgumentMatcher
	{
		public override bool TryConvertArgument(string argument, Type argumentType, DevConsole devConsole, out object converted)
		{
			if (argumentType != typeof(string))
			{
				converted = null;
				return false;
			}
			var success = devConsole.Parser.IsName(argument);
			converted	= argument;
			return success;
		}
	}

	/// <summary>Converts arguments to string objects.</summary>
	public class StringMatcher : DevConsoleArgumentMatcher
	{
		public override bool TryConvertArgument(string argument, Type argumentType, DevConsole devConsole, out object converted)
		{
			if (argumentType != typeof(string))
			{
				converted = null;
				return false;
			}
			converted = argument;
			return true;
		}
	}

	/// <summary>Converts arguments to Type objects.</summary>
	public class TypeMatcher : DevConsoleArgumentMatcher
	{
		public override bool TryConvertArgument(string argument, Type argumentType, DevConsole devConsole, out object converted)
		{
			if (argumentType != typeof(Type))
			{
				converted = null;
				return false;
			}
			var success = devConsole.Parser.ParseType(argument, true, true, out var result);
			converted	= result;
			return success;
		}
	}

	/// <summary>Converts arguments to objects mapped in DevConsole's InstanceMappers.</summary>
	public class InstanceMatcher : DevConsoleArgumentMatcher
	{
		public override bool TryConvertArgument(string argument, Type argumentType, DevConsole devConsole, out object converted)
		{
			var mapper = devConsole.InstanceMappers.FirstOrDefault(m => m.InstanceType.IsAssignableFrom(argumentType));
			if (mapper != null)
			{
				return mapper.GetObjectByInstanceID(argument, out converted);
			}
			converted = null;
			return false;
		}
	}
}
