using UnityEngine;

namespace com.SolePilgrim.Instancing.Unity
{
	/// <summary>Component that stores and manages an InstanceMapper and persists over the entire runtime.</summary>
	abstract public class InstanceMapperComponent : MonoBehaviour
    {
		/// <summary>The managed InstanceMapper.</summary>
		public abstract InstanceMapper InstanceMapper { get; }
		/// <summary>Seconds between automated UpdateMapping calls. A value of 0 or lower disables automated updating.</summary>
		[field: SerializeField, Tooltip("Seconds between automated UpdateMapping calls. A value of 0 or lower disables automated updating.")]
		public float MappingUpdatePeriod { get; private set; } = 0;


		protected virtual void Awake()
		{
			DontDestroyOnLoad(gameObject);
			if (MappingUpdatePeriod > 0)
			{
				InvokeRepeating(nameof(UpdateMapping), MappingUpdatePeriod, MappingUpdatePeriod);
			}
		}

		/// <summary>Update the InstanceMapper.</summary>
		public void UpdateMapping()
		{
			InstanceMapper?.UpdateMapping();
		}
	}
}
