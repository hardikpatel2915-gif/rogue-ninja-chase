using System;
using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.Model;
using RogueNinjaChase.Player.View;

namespace RogueNinjaChase.Player.Presenter
{
    public sealed class PlayerMotorPresenter
    {
        readonly IPlayerMotorModel _model;
        readonly IPlayerMotorView _view;
        readonly IMoveInputSource _input;
        readonly IAimSource _aim;

        public PlayerMotorPresenter(
            IPlayerMotorModel model,
            IPlayerMotorView view,
            IMoveInputSource input,
            IAimSource aim)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _aim = aim ?? throw new ArgumentNullException(nameof(aim));
        }

        public void Tick()
        {
            MoveInput input = _input.Read();
            MotorOutput output = _model.Evaluate(in input, _aim.Forward, _aim.Right);
            _view.SetPlanarVelocity(output.PlanarVelocity);
            _view.SetCrouching(output.IsCrouching);
        }
    }
}
