using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace com.SolePilgrim.Instancing
{
	/// <summary>Maps all currently loaded objects with an InstanceID to their IDs.</summary>
	abstract public class InstanceMapper
	{
		public Type KeyType				{ get; protected set; }
		public Type InstanceType		{ get; protected set; }
		public Regex InstanceIDRegex	{ get; protected set; }

		/// <summary>Checks if the given string is a valid InstanceID.</summary>
		public bool IsInstanceID(string text, out string id)
		{
			id = InstanceIDRegex.Match(text).Groups["id"].Value;
			return !string.IsNullOrEmpty(id);
		}

		/// <summary>Tries to find a currently loaded object by the given id.</summary>
		/// <param name="id">ID of an Instance. Identifiers get filtered out.</param>
		/// <returns>The found instance, if any.</returns>
		public abstract object GetObjectByInstanceID(string id);
		/// <summary>Tries to find a currently loaded object by the given id.</summary>
		/// <param name="id">ID of an Instance. Identifiers get filtered out.</param>
		/// <param name="result">The found instance, if any.</param>
		/// <returns>True if a matching object was found, else false.</returns>
		public abstract bool GetObjectByInstanceID(string id, out object result);
		/// <summary>Force the Mapper to update itself. Use sporadically, as this goes over all loaded in objects.</summary>
		public abstract void UpdateMapping();
		/// <summary>Force the mapping to clear itself.</summary>
		public abstract void ClearMapping();
		/// <summary>Returns all the currently mapped instances and keys.</summary>
		public abstract Dictionary<object, object> GetMappings();
	}

    /// <summary>Maps all currently loaded objects of type T2 to per-session unique InstanceIDs of type T1.</summary>
    abstract public class InstanceMapper<T1, T2> : InstanceMapper where T2 : class
    {
        protected readonly Dictionary<T1, T2> _objectsMap = new Dictionary<T1, T2>();
        protected readonly List<T1> _keysCache = new List<T1>();
        protected T2 _objectCache;


        public InstanceMapper()
        {
			KeyType			= typeof(T1);
			InstanceType	= typeof(T2);
            UpdateMapping();
        }

		public override object GetObjectByInstanceID(string id)
		{
			var rawId		= IsInstanceID(id, out var result) ? result : id;
			var key			= ConvertInstanceID(rawId);
			var hasObject	= _objectsMap.TryGetValue(key, out _objectCache);
			//Map may be out of date, so update it.
			if (!hasObject || _objectCache == null)
			{
				UpdateMapping();
			}
			return _objectsMap[key];
		}

		public override bool GetObjectByInstanceID(string id, out object result)
		{
			result = GetObjectByInstanceID(id);
			return result != null;
		}

		public override void ClearMapping()
		{
			_objectsMap.Clear();
		}

		public override Dictionary<object, object> GetMappings()
		{
			var result = new Dictionary<object, object>();
			foreach(var kvp in _objectsMap)
			{
				result.Add(kvp.Key, kvp.Value);
			}
			return result;
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
			var objects = FindInstances();
			foreach (var obj in objects)
			{
				var id = GetInstanceIDFromObject(obj);
				if (!_objectsMap.ContainsKey(id))
				{
					_objectsMap.Add(id, obj);
				}
			}
		}

		/// <summary>Gets a per-session unique instance id for the given object.</summary>
		public abstract T1 GetInstanceIDFromObject(T2 obj);
		/// <summary>Converts given raw InstanceID string to InstanceID.</summary>
		protected abstract T1 ConvertInstanceID(string id);
		/// <summary>Receive all instances to be registered in UpdateMapping.</summary>
		protected abstract T2[] FindInstances();
	}
}
