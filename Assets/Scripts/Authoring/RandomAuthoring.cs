using Unity.Entities;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Authoring
{
    public struct RandomComponent : IComponentData
    {
        public Random random;
    }

    public class RandomAuthoring : MonoBehaviour
    {
        public int seed;

        private class RandomBaker : Baker<RandomAuthoring>
        {
            public override void Bake(RandomAuthoring authoring)
            {
                AddComponent(new RandomComponent()
                {
                    random = new Random((uint)authoring.seed)
                });
            }
        }
    }
}