using System;
using UnityEngine;

namespace com.SolePilgrim.DevConsole.Unity
{
    static public class UnityDemoDevConsoleCommands
    {
        [ConsoleMacro("Player", new Type[] { })]
        static public GameObject GetPlayer()
        {
            return GameObject.FindWithTag("player");
        }
    }
}
