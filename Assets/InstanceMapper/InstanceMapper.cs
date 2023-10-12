using System.Collections.Generic;

namespace com.SolePilgrim.Instancing
{
    /// <summary>Maps all currently loaded objects of type T to per-session unique InstanceIDs.</summary>
    abstract public class InstanceMapper<T> where T : class
    {
        protected readonly Dictionary<int, T> _objectsMap = new Dictionary<int, T>();
        protected readonly List<int> _keysCache = new List<int>();
        protected T _objectCache;


        public InstanceMapper()
        {
            UpdateMapping();
        }

        /// <summary>Tries to find a currently loaded object by the given id.</summary>
        public T GetObjectByInstanceID(int id)
        {
            var hasObject = _objectsMap.TryGetValue(id, out _objectCache);
            //Map may be out of date, so update it.
            if (!hasObject || _objectCache == null)
            {
                UpdateMapping();
            }
            return _objectsMap[id];
        }

        /// <summary>Gets a per-session unique instance id for the given object.</summary>
        public abstract int GetInstanceIDFromObject(T obj);

        /// <summary>Force the Mapper to update itself. Use sporadically, as this goes over all loaded in objects.</summary>
        public abstract void UpdateMapping();

        /// <summary>Force the mapping to completely clear out.</summary>
        public void ClearMapping()
        {
            _objectsMap.Clear();
        }
    }
}