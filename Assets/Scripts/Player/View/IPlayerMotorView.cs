using RogueNinjaChase.Player.Model;
using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    public interface IPlayerMotorView
    {
        float StandingHeight { get; }
        float StandingRadius { get; }
        float StandingCenterY { get; }
        float VerticalSpeed { get; }

        void SetPlanarVelocity(Vector3 planarVelocity);
        void ApplyCapsule(in CapsulePose pose);
        void ApplyJump(in JumpCommand command);
        bool ProbeGround();
    }
}
