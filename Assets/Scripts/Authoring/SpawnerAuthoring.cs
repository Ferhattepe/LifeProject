using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Authoring
{
    public struct Spawner : IComponentData
    {
        public Entity Prefab;
        public float3 SpawnAreaCenter;
        public float MaxSpawnRange;
        public float NextSpawnTime;
        public float SpawnRate;
    }

    public class SpawnerAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public float spawnRate;
        public float spawnRange;

        private class SpawnerAuthoringBaker : Baker<SpawnerAuthoring>
        {
            public override void Bake(SpawnerAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new Spawner()
                {
                    Prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
                    SpawnAreaCenter = authoring.transform.position,
                    NextSpawnTime = 0.0f,
                    SpawnRate = authoring.spawnRate,
                    MaxSpawnRange = authoring.spawnRange
                });
            }
        }
    }
}