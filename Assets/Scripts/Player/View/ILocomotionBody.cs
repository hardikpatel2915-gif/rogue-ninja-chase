using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    public interface ILocomotionBody
    {
        bool IsCrouching { get; }
        Vector3 PlanarVelocity { get; }
        float WalkSpeed { get; }
        bool IsJumping { get; }
    }
}
