using UnityEngine;

namespace RogueNinjaChase.Ninja.Vision
{
    /// <summary>
    /// Angle and distance only. A wall check is separate, so this stays free of physics.
    /// </summary>
    public static class VisionMath
    {
        const float DistanceEpsilonSqr = 1e-8f;

        public static float CosHalfAngle(float viewAngleDegrees)
        {
            return Mathf.Cos(viewAngleDegrees * 0.5f * Mathf.Deg2Rad);
        }

        public static ConeTest TestCone(
            Vector3 origin,
            Vector3 forward,
            Vector3 target,
            float cosHalfAngle,
            float maxDistanceSqr)
        {
            Vector3 offset = target - origin;
            float sqr = offset.sqrMagnitude;
            if (sqr > maxDistanceSqr || sqr < DistanceEpsilonSqr)
                return new ConeTest(false, 0f, Vector3.zero);

            float distance = Mathf.Sqrt(sqr);
            Vector3 direction = offset / distance;
            // cos(half angle) is cached by the caller. Dot avoids Acos on the hot path.
            float dot = Vector3.Dot(forward, direction);
            return new ConeTest(dot >= cosHalfAngle, distance, direction);
        }
    }
}
