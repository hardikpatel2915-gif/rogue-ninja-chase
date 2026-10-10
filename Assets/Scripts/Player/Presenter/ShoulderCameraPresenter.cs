using System;
using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;

namespace RogueNinjaChase.Player.Presenter
{
    public sealed class ShoulderCameraPresenter
    {
        readonly IShoulderCameraModel _model;
        readonly ILookSource _look;

        public ShoulderCameraPresenter(IShoulderCameraModel model, ILookSource look)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
            _look = look ?? throw new ArgumentNullException(nameof(look));
        }

        public CameraPose Tick()
        {
            LookDelta look = _look.Read();
            return _model.AddLook(in look);
        }
    }
}
