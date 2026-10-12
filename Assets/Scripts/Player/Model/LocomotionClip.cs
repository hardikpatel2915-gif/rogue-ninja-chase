using UnityEngine;

namespace RogueNinjaChase.Player.Model
{
    public enum LocomotionClip
    {
        Idle = 0,
        Walk = 1,
        Sprint = 2,
        CrouchIdle = 3,
        CrouchWalk = 4,
        Jump = 5,
        Throw = 6,
        StrafeLeft = 7,
        StrafeRight = 8,
        StrafeLeftSprint = 9,
        StrafeRightSprint = 10
    }

    public enum StrafeSide
    {
        None = 0,
        Left = 1,
        Right = 2
    }

    /// <summary>
    /// Which clip to play. Throw and jump hold until they finish. Crouch never uses the stand clips.
    /// Strafe is used when the move is more sideways than forward.
    /// </summary>
    public static class LocomotionSelect
    {
        const float MoveEpsilon = 0.05f;
        const float AxisEpsilonSqr = 0.0001f;

        public static StrafeSide Side(Vector3 velocity, Vector3 forward, Vector3 right)
        {
            velocity.y = 0f;
            forward.y = 0f;
            right.y = 0f;
            if (forward.sqrMagnitude < AxisEpsilonSqr || right.sqrMagnitude < AxisEpsilonSqr)
                return StrafeSide.None;

            forward /= Mathf.Sqrt(forward.sqrMagnitude);
            right /= Mathf.Sqrt(right.sqrMagnitude);
            float lateral = Vector3.Dot(velocity, right);
            float ahead = Vector3.Dot(velocity, forward);
            if (Mathf.Abs(lateral) <= Mathf.Abs(ahead))
                return StrafeSide.None;

            return lateral >= 0f ? StrafeSide.Right : StrafeSide.Left;
        }

        public static LocomotionClip Select(
            bool crouching,
            float planarSpeed,
            float walkSpeed,
            bool jumping,
            bool throwing,
            StrafeSide strafe = StrafeSide.None)
        {
            if (throwing)
                return LocomotionClip.Throw;
            if (jumping)
                return LocomotionClip.Jump;
            if (crouching)
                return planarSpeed > MoveEpsilon ? LocomotionClip.CrouchWalk : LocomotionClip.CrouchIdle;
            if (planarSpeed > MoveEpsilon && strafe == StrafeSide.Left)
                return planarSpeed > walkSpeed + MoveEpsilon ? LocomotionClip.StrafeLeftSprint : LocomotionClip.StrafeLeft;
            if (planarSpeed > MoveEpsilon && strafe == StrafeSide.Right)
                return planarSpeed > walkSpeed + MoveEpsilon ? LocomotionClip.StrafeRightSprint : LocomotionClip.StrafeRight;
            if (planarSpeed > walkSpeed + MoveEpsilon)
                return LocomotionClip.Sprint;
            if (planarSpeed > MoveEpsilon)
                return LocomotionClip.Walk;
            return LocomotionClip.Idle;
        }
    }
}
