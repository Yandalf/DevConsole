using com.SolePilgrim.Instancing;
using UnityEngine;

/// <summary>Component that stores and manages an InstanceMapper and persists over the entire runtime.</summary>
public class UnityObjectInstanceMapperComponent : MonoBehaviour
{
    /// <summary>Active InstanceMapper.</summary>
    public InstanceMapper<int, GameObject> InstanceMapper { get; private set; }
    /// <summary>"Seconds between automated Mapping Updates. A value below 0 disables automated updating."</summary>
    [field: SerializeField, Tooltip("Seconds between automated Mapping Updates. A value below 0 disables automated updating.")]
    public float MappingUpdatePeriod { get; private set; } = -1;


    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        InstanceMapper = new GameObjectInstanceMapper();
        if(MappingUpdatePeriod >= 0)
        {
            InvokeRepeating(nameof(UpdateMapping), MappingUpdatePeriod, MappingUpdatePeriod);
        }
    }

    private void UpdateMapping()
    {
        InstanceMapper?.UpdateMapping();
    }
}
