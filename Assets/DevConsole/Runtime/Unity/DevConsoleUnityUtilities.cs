using System.Linq;
using UnityEngine;

namespace com.SolePilgrim.DevConsole.Unity
{
	/// <summary>Contains methods for the Dev Console that interface with Unity's API.</summary>
	static public class DevConsoleUnityUtilities
	{
		[ConsoleMethod]
		static public string Log(GameObject obj)
		{
			Debug.Log(obj);
			return obj.ToString();
		}

		[ConsoleMethod]
		static public string LogComponents(GameObject obj)
		{
			var components = obj.GetComponents<Component>();
			var str = $"{obj}:\n{string.Join("\n", components.Select(c => $"-{c.GetType().FullName}"))}";
			Debug.Log(str);
			return str;
		}

		[ConsoleMethod]
		static public GameObject GetPlayer()
		{
			return GameObject.FindWithTag("Player");
		}

		[ConsoleMethod]
		static public string Foo(GameObject obj)
		{
			return $"foo gameobject:{obj}";
		}
	}
}
