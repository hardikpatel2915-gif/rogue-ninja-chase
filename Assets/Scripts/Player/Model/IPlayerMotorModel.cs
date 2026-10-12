using UnityEngine;

namespace RogueNinjaChase.Player.Model
{
    public interface IPlayerMotorModel
    {
        MotorOutput Evaluate(in MoveInput input, Vector3 cameraForward, Vector3 cameraRight);
        JumpCommand AdvanceJump(bool requested, bool grounded, float verticalSpeed, float deltaTime);
    }
}
