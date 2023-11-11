using UnityEngine;

namespace com.SolePilgrim.Instancing.Unity
{
	[DisallowMultipleComponent, AddComponentMenu("SolePilgrim/InstanceMapper/ObjectInstanceMapper Component")]
	/// <summary>InstanceMapperComponent for Unity Objects.</summary>
	public class UnityObjectInstanceMapperComponent : InstanceMapperComponent
	{
		/// <summary>Active InstanceMapper.</summary>
		public InstanceMapper<int, Object> ObjectInstanceMapper { get; private set; }
		public override InstanceMapper InstanceMapper => ObjectInstanceMapper;


		protected override void Awake()
		{
			base.Awake();
			ObjectInstanceMapper = new UnityObjectInstanceMapper();
		}
	}
}
