using com.SolePilgrim.Instancing;
using System;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.SolePilgrim.DevConsole.Unity
{
	/// <summary>Contains methods for the Dev Console that interface with Unity's API.</summary>
	static public class DevConsoleUnityUtilities
	{
		[ConsoleMacro("GetByInstanceID", new Type[] { typeof(int) })]
		static public Object GetByInstanceID(InstanceMapper<Object> mapper, int instanceId)
		{
			return mapper.GetObjectByInstanceID(instanceId);
		}

		[ConsoleMethod]
		static public void Log(this Object obj)
		{
			Debug.Log(obj);
		}

		[ConsoleMethod]
		static public void LogComponents(this GameObject obj)
		{
			var components = obj.GetComponents<Component>();
			Debug.Log($"{obj}:\n{string.Join("\n-", components.Select(c => c.GetType().FullName))}");
		}
	}
}
