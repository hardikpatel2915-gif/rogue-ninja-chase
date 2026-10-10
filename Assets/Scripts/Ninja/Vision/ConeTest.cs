using UnityEngine;

namespace RogueNinjaChase.Ninja.Vision
{
    /// <summary>
    /// Inside is angle and distance. Direction is for the wall ray that follows.
    /// </summary>
    public readonly struct ConeTest
    {
        public readonly bool Inside;
        public readonly float Distance;
        public readonly Vector3 Direction;

        public ConeTest(bool inside, float distance, Vector3 direction)
        {
            Inside = inside;
            Distance = distance;
            Direction = direction;
        }
    }
}
