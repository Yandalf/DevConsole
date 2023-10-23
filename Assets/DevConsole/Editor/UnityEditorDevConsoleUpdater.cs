using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;

namespace com.SolePilgrim.DevConsole.Unity
{
	/// <summary>Wraps DevConsoleCommandSearcher for the Unity Editor.</summary>
	static internal class UnityEditorDevConsoleUpdater
	{
		//TODO find a way to ensure this only runs when scripts are recompiled. Now any kind of (re)import triggers this, which will be very inconvenient for devs.
		//[UnityEditor.Callbacks.DidReloadScripts]
		//static private void OnScriptReload()
		//{
		//	Debug.Log($"IsPlaying: {EditorApplication.isPlayingOrWillChangePlaymode} IsCompiling: {EditorApplication.isCompiling} IsUpdating: {EditorApplication.isUpdating}");
		//	if (EditorApplication.isPlayingOrWillChangePlaymode) //Don't run this on play mode
		//	{
		//		return;
		//	}
		//	if (EditorApplication.isCompiling || EditorApplication.isUpdating)
		//	{
		//		EditorApplication.delayCall += FindAllConsoleCommands;
		//		return;
		//	}
		//}

		[MenuItem("SolePilgrim/DevConsole/Update Console Commands")]
		static private void FindAllConsoleCommands()
		{
			DevConsoleCommandSearcher.FindAllConsoleCommands(out var methods, out var macros);
			Debug.Log($"{nameof(UnityEditorDevConsoleUpdater)}.{nameof(FindAllConsoleCommands)}" +
				$"\nFound methods: {methods.Count()}\n{string.Join("\n", methods.Select(m => $"-{m.Name}"))}" +
				$"\nFound macros: {macros.Count()}\n{string.Join("\n", macros.Select(m => $"-{m.Name}"))}");
			//The methods found here need to become accessible for the Console to call upon the correct objects.
			var allSerialized = DevConsoleCommandSearcher.ConsoleCommandsToString(methods, macros, Newtonsoft.Json.Formatting.Indented); //TODO remove this indented parameter
			Debug.Log(allSerialized);
			CreateOrEditTextAsset(allSerialized);
		}

		static private void CreateOrEditTextAsset(string serializedText)
        {
			var dirPath = Path.Combine(Application.dataPath, "Resources", "DevConsole");
			var filePath = Path.Combine(dirPath, "ConsoleCommands.txt");
			if (!Directory.Exists(dirPath))
            {
				Directory.CreateDirectory(dirPath);
            }
			if (!File.Exists(filePath))
            {
				File.Create(filePath).Close();
            }
			File.WriteAllText(filePath, serializedText);
			AssetDatabase.Refresh();
        }
	}
}