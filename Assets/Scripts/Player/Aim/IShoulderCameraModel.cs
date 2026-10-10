using RogueNinjaChase.Player.Input;

namespace RogueNinjaChase.Player.Aim
{
    public interface IShoulderCameraModel
    {
        CameraPose Pose { get; }
        CameraPose AddLook(in LookDelta look);
    }
}
