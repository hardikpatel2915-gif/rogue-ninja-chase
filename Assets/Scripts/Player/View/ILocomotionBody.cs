using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    public interface ILocomotionBody
    {
        bool IsCrouching { get; }
        Vector3 PlanarVelocity { get; }
        Vector3 BodyForward { get; }
        Vector3 BodyRight { get; }
        float WalkSpeed { get; }
        bool IsJumping { get; }
    }
}
