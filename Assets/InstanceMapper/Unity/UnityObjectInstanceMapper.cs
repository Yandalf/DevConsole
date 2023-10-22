using com.SolePilgrim.DevConsole;
using System.Text.RegularExpressions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.SolePilgrim.Instancing
{
	/// <summary>Unity implementation of InstanceMapper.</summary>
	public class UnityObjectInstanceMapper : InstanceMapper<int, Object>
    {
		public UnityObjectInstanceMapper()
		{
			InstanceIDRegex = new Regex($"^i:(?<id>{DevConsoleUtilities.IntegerRegex})$", RegexOptions.IgnoreCase);
		}

        public override int GetInstanceIDFromObject(Object obj)
        {
            return obj.GetInstanceID();
        }

		protected override int ConvertInstanceID(string id)
		{
			return int.Parse(id);
		}

		protected override Object[] FindInstances()
		{
			return Resources.FindObjectsOfTypeAll<Object>();
		}
	}
}
