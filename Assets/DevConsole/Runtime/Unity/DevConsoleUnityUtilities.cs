using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.SolePilgrim.DevConsole.Unity
{
	/// <summary>Contains methods for the Dev Console that interface with Unity's API.</summary>
	static public class DevConsoleUnityUtilities
	{
		[ConsoleMethod]
		static public void Log(Object obj)
		{
			Debug.Log(obj);
		}

		[ConsoleMethod]
		static public void LogComponents(GameObject obj)
		{
			var components = obj.GetComponents<Component>();
			Debug.Log($"{obj}:\n{string.Join("\n-", components.Select(c => c.GetType().FullName))}");
		}
	}
}
