using RogueNinjaChase.Player.Model;
using RogueNinjaChase.Player.View;

namespace RogueNinjaChase.Player.Presenter
{
    /// <summary>
    /// Picks the locomotion clip from the body, and holds throw until that state finishes.
    /// </summary>
    public sealed class PlayerLocomotionPresenter
    {
        readonly IPlayerLocomotionView _view;
        readonly ILocomotionBody _motor;
        LocomotionClip _current = LocomotionClip.Idle;
        bool _throwing;
        bool _replay;

        public PlayerLocomotionPresenter(IPlayerLocomotionView view, ILocomotionBody motor)
        {
            _view = view;
            _motor = motor;
        }

        public bool Throwing => _throwing;

        public void NotifyThrow()
        {
            _throwing = true;
            _replay = true;
        }

        public void Tick(bool aimStrafe = false)
        {
            if (_motor == null || _view == null)
                return;

            if (_throwing && !_view.ThrowInTransition && _view.InThrowState && _view.ThrowNormalizedTime >= 1f)
                _throwing = false;

            StrafeSide strafe = StrafeSide.None;
            if (aimStrafe)
            {
                strafe = LocomotionSelect.Side(
                    _motor.PlanarVelocity,
                    _motor.BodyForward,
                    _motor.BodyRight);
            }

            LocomotionClip clip = LocomotionSelect.Select(
                _motor.IsCrouching,
                _motor.PlanarVelocity.magnitude,
                _motor.WalkSpeed,
                _motor.IsJumping,
                _throwing,
                strafe);

            if (!_replay && clip == _current)
                return;

            _replay = false;
            _current = clip;
            _view.Play(clip);
        }
    }
}
