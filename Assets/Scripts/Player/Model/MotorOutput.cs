using UnityEngine;

namespace RogueNinjaChase.Player.Model
{
    public readonly struct MotorOutput
    {
        public readonly Vector3 PlanarVelocity;
        public readonly bool IsCrouching;
        public readonly bool Jump;

        public MotorOutput(Vector3 planarVelocity, bool isCrouching, bool jump)
        {
            PlanarVelocity = planarVelocity;
            IsCrouching = isCrouching;
            Jump = jump;
        }
    }
}
