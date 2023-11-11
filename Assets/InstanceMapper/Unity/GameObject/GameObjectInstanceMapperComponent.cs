using UnityEngine;

namespace com.SolePilgrim.Instancing.Unity
{
	[DisallowMultipleComponent, AddComponentMenu("SolePilgrim/InstanceMapper/GameObjectInstanceMapper Component")]
    /// <summary>InstanceMapperComponent for GameObjects.</summary>
    public class GameObjectInstanceMapperComponent : InstanceMapperComponent
    {
        /// <summary>Active InstanceMapper.</summary>
        public InstanceMapper<int, GameObject> GameObjectInstanceMapper { get; private set; }
		public override InstanceMapper InstanceMapper => GameObjectInstanceMapper;


		protected override void Awake()
        {
            base.Awake();
			GameObjectInstanceMapper = new GameObjectInstanceMapper();
        }
	}
}
