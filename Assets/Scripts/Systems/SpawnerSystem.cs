using Authoring;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    public partial struct SpawnerSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = GetEntityCommandBuffer(ref state);
            var random = SystemAPI.GetSingletonRW<RandomComponent>();

            new ProcessSpawnerJob()
            {
                ElapsedTime = SystemAPI.Time.ElapsedTime,
                Ecb = ecb,
                Random = random
            }.ScheduleParallel();
        }

        private EntityCommandBuffer.ParallelWriter GetEntityCommandBuffer(ref SystemState state)
        {
            var ecbSingleton = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
            return ecb.AsParallelWriter();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }
    }

    [BurstCompile]
    public partial struct ProcessSpawnerJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter Ecb;
        public double ElapsedTime;

        [NativeDisableUnsafePtrRestriction]
        public RefRW<RandomComponent> Random;

        private void Execute([ChunkIndexInQuery] int chunkIndex, ref Spawner spawner)
        {
            if (spawner.NextSpawnTime < ElapsedTime)
            {
                var newEntity = Ecb.Instantiate(chunkIndex, spawner.Prefab);
                var randomVector = Random.ValueRW.random.NextFloat3Direction();
                var range = Random.ValueRW.random.NextFloat(0f, 1f) * spawner.MaxSpawnRange;
                var spawnPoint = spawner.SpawnAreaCenter + randomVector * range;

                Ecb.SetComponent(chunkIndex, newEntity,
                    LocalTransform.FromPosition(spawnPoint));
                spawner.NextSpawnTime = (float)ElapsedTime + spawner.SpawnRate;
            }
        }
    }
}