using UnityEngine;

namespace RogueNinjaChase.Player
{
    public interface IPlayerMotorView
    {
        void SetPlanarVelocity(Vector3 planarVelocity);
        void SetCrouching(bool crouching);
    }
}
