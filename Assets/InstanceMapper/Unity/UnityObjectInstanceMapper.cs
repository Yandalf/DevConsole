using com.SolePilgrim.DevConsole;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace com.SolePilgrim.Instancing
{
    /// <summary>Unity implementation of InstanceMapper.</summary>
    public class UnityObjectInstanceMapper : InstanceMapper<int, Object>
    {
        public override int GetInstanceIDFromObject(Object obj)
        {
            return obj.GetInstanceID();
        }

		public override bool IsInstanceID(string id)
		{
			return DevConsoleUtilities.IntegerRegexPattern.IsMatch(id);
		}

		public override void UpdateMapping()
        {
            //Clear out objects that no longer exist.
            _keysCache.AddRange(_objectsMap.Where(p => p.Value == null).Select(p => p.Key));
            foreach (var key in _keysCache)
            {
                _objectsMap.Remove(key);
            }
            _keysCache.Clear();
            //Add new objects
            var objects = Resources.FindObjectsOfTypeAll<Object>();
            foreach (var obj in objects)
            {
                var id = GetInstanceIDFromObject(obj);
                if (!_objectsMap.ContainsKey(id))
                {
                    _objectsMap.Add(id, obj);
                }
            }
            //Debug.Log($"Updated {nameof(UnityObjectInstanceMapper)}, Elements:{_objectsMap.Count}.\n{string.Join("\n", _objectsMap.Select(p => $"{p.Key}-{p.Value}"))}");
        }

		protected override int ConvertInstanceIDImplementation(string id)
		{
			return int.Parse(id);
		}
	}
}
