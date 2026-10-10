using UnityEngine;

namespace RogueNinjaChase.Player
{
    public readonly struct MotorOutput
    {
        public readonly Vector3 PlanarVelocity;
        public readonly bool IsCrouching;

        public MotorOutput(Vector3 planarVelocity, bool isCrouching)
        {
            PlanarVelocity = planarVelocity;
            IsCrouching = isCrouching;
        }
    }
}
