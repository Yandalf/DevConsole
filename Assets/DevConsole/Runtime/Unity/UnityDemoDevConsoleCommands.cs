using UnityEngine;

namespace com.SolePilgrim.DevConsole.Unity
{
    static public class UnityDemoDevConsoleCommands
    {
        [ConsoleMethod]
        static public GameObject GetPlayer()
        {
            return GameObject.FindWithTag("player");
        }

		[ConsoleMethod]
		static public void Foo(GameObject obj)
		{

		}
    }
}
