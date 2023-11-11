using com.SolePilgrim.DevConsole;
using System.Text.RegularExpressions;
using UnityEngine;

namespace com.SolePilgrim.Instancing.Unity
{
	/// <summary>GameObject implementation of InstanceMapper.</summary>
	public class GameObjectInstanceMapper : InstanceMapper<int, GameObject>
	{
		public GameObjectInstanceMapper()
		{
			InstanceIDRegex = new Regex($"^i:(?<id>{DevConsoleUtilities.IntegerRegex})$", RegexOptions.IgnoreCase);
		}

		public override int GetInstanceIDFromObject(GameObject obj)
		{
			return obj.GetInstanceID();
		}

		protected override int ConvertInstanceID(string id)
		{
			return int.Parse(id);
		}

		protected override GameObject[] FindInstances()
		{
			return Resources.FindObjectsOfTypeAll<GameObject>();
		}
	}
}
