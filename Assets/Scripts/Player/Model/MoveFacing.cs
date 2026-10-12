using UnityEngine;

namespace RogueNinjaChase.Player.Model
{
    /// <summary>
    /// Body yaw for the far camera. Idle keeps the current facing. Movement turns toward the velocity.
    /// </summary>
    public static class MoveFacing
    {
        const float MoveEpsilonSqr = 0.0001f;

        public static float YawDegrees(float currentYaw, Vector3 planarVelocity, float turnSpeed, float deltaTime)
        {
            if (planarVelocity.sqrMagnitude <= MoveEpsilonSqr)
                return currentYaw;

            float target = Mathf.Atan2(planarVelocity.x, planarVelocity.z) * Mathf.Rad2Deg;
            return Mathf.MoveTowardsAngle(currentYaw, target, turnSpeed * deltaTime);
        }
    }
}
