using System;
using System.Collections.Generic;

namespace com.SolePilgrim.Instancing
{
	/// <summary>Maps all currently loaded objects with an InstanceID to their IDs.</summary>
	abstract public class InstanceMapper
	{
		public Type InstanceType { get; protected set; }

		/// <summary>Tries to find a currently loaded object by the given id.</summary>
		public abstract object GetObjectByInstanceID(string id);
		/// <summary>Force the Mapper to update itself. Use sporadically, as this goes over all loaded in objects.</summary>
		public abstract void UpdateMapping();
		/// <summary>Force the mapping to completely clear out.</summary>
		public abstract void ClearMapping();
		/// <summary>Checks if the given string is a valid InstanceID.</summary>
		public abstract bool IsInstanceID(string id);
	}

    /// <summary>Maps all currently loaded objects of type T2 to per-session unique InstanceIDs of type T1.</summary>
    abstract public class InstanceMapper<T1, T2> : InstanceMapper where T2 : class
    {
        protected readonly Dictionary<T1, T2> _objectsMap = new Dictionary<T1, T2>();
        protected readonly List<int> _keysCache = new List<int>();
        protected T2 _objectCache;


        public InstanceMapper()
        {
			InstanceType = typeof(T2);
            UpdateMapping();
        }

		public override object GetObjectByInstanceID(string id)
		{
			var key = ConvertInstanceID(id);
			var hasObject = _objectsMap.TryGetValue(key, out _objectCache);
			//Map may be out of date, so update it.
			if (!hasObject || _objectCache == null)
			{
				UpdateMapping();
			}
			return _objectsMap[key];
		}

		public override void ClearMapping()
		{
			_objectsMap.Clear();
		}

		/// <summary>Converts a string to a valid InstanceID.</summary>
		public T1 ConvertInstanceID(string id)
		{
			if (!IsInstanceID(id))
			{
				throw new ArgumentException($"{id} is not a valid InstanceID.", nameof(id));
			}
			return ConvertInstanceIDImplementation(id);
		}

		/// <summary>Gets a per-session unique instance id for the given object.</summary>
		public abstract T1 GetInstanceIDFromObject(T2 obj);

		/// <summary>Implementation of GoncertInstanceID.</summary>
		protected abstract T1 ConvertInstanceIDImplementation(string id);
	}
}
