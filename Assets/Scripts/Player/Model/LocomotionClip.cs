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
        Throw = 6
    }

    /// <summary>
    /// Which clip to play. Throw and jump hold until they finish. Crouch never uses the stand clips.
    /// </summary>
    public static class LocomotionSelect
    {
        const float MoveEpsilon = 0.05f;

        public static LocomotionClip Select(
            bool crouching,
            float planarSpeed,
            float walkSpeed,
            bool jumping,
            bool throwing)
        {
            if (throwing)
                return LocomotionClip.Throw;
            if (jumping)
                return LocomotionClip.Jump;
            if (crouching)
                return planarSpeed > MoveEpsilon ? LocomotionClip.CrouchWalk : LocomotionClip.CrouchIdle;
            if (planarSpeed > walkSpeed + MoveEpsilon)
                return LocomotionClip.Sprint;
            if (planarSpeed > MoveEpsilon)
                return LocomotionClip.Walk;
            return LocomotionClip.Idle;
        }
    }
}
