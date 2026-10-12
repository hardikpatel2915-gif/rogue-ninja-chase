using RogueNinjaChase.Player.Model;

namespace RogueNinjaChase.Player.View
{
    public interface IPlayerLocomotionView
    {
        void Play(LocomotionClip clip);
        bool ThrowInTransition { get; }
        bool InThrowState { get; }
        float ThrowNormalizedTime { get; }
    }
}
