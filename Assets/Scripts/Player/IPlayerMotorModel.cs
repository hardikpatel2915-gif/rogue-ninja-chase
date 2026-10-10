using UnityEngine;

namespace RogueNinjaChase.Player
{
    public interface IPlayerMotorModel
    {
        MotorOutput Evaluate(in MoveInput input, Vector3 cameraForward, Vector3 cameraRight);
    }
}
