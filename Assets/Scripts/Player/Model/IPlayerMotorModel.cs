using UnityEngine;

namespace RogueNinjaChase.Player.Model
{
    public interface IPlayerMotorModel
    {
        MotorOutput Evaluate(in MoveInput input, Vector3 cameraForward, Vector3 cameraRight);
    }
}
