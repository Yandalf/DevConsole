using UnityEngine;
using UnityEditor;
using System.Linq;

namespace com.SolePilgrim.DevConsole.Unity
{
	/// <summary>Wraps DevConsoleCommandSearcher for the Unity Editor.</summary>
	static internal class UnityEditorDevConsoleUpdater
	{
		//[UnityEditor.Callbacks.DidReloadScripts]
		static private void OnScriptReload()
		{
			Debug.Log($"IsPlaying: {EditorApplication.isPlayingOrWillChangePlaymode} IsCompiling: {EditorApplication.isCompiling} IsUpdating: {EditorApplication.isUpdating}");
			if (EditorApplication.isPlayingOrWillChangePlaymode) //Don't run this on play mode
			{
				return;
			}
			if (EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				EditorApplication.delayCall += FindAllConsoleCommands;
				return;
			}
		}

		[MenuItem("SolePilgrim/DevConsole/Update Console Commands")]
		static private void FindAllConsoleCommands()
		{
			DevConsoleCommandSearcher.FindAllConsoleCommands(out var methods, out var macros, out var types);
			Debug.Log($"{nameof(UnityEditorDevConsoleUpdater)}.{nameof(FindAllConsoleCommands)}" +
				$"\nFound methods: {methods.Count()}\n{string.Join("\n", methods.Select(m => $"-{m.Name}"))}" +
				$"\nFound Macros: {macros.Count()}\n{string.Join("\n", macros.Select(m => $"-{m.Name}"))}" + 
				$"\nFound Classes: {types.Count()}\n{string.Join("\n", types.Select(t => $"-{t.Name}"))}");
			//The methods found here need to become accessible for the Console to call upon the correct objects.
		}
	}
}