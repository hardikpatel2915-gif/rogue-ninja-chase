using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    public readonly struct NinjaSense
    {
        public readonly bool CanSeePlayer;
        public readonly Vector3 PlayerPosition;
        public readonly bool HasNoise;
        public readonly Vector3 NoisePosition;
        public readonly bool AtDestination;

        public NinjaSense(
            bool canSeePlayer,
            Vector3 playerPosition,
            bool hasNoise,
            Vector3 noisePosition,
            bool atDestination)
        {
            CanSeePlayer = canSeePlayer;
            PlayerPosition = playerPosition;
            HasNoise = hasNoise;
            NoisePosition = noisePosition;
            AtDestination = atDestination;
        }
    }
}
