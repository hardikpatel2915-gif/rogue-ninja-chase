using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;

namespace RogueNinjaChase.Player.Presenter
{
    /// <summary>
    /// Applies mouse look and toggles shoulder and follow framing.
    /// </summary>
    public sealed class ShoulderCameraPresenter
    {
        readonly IShoulderCameraModel _model;
        readonly ILookSource _look;
        CameraViewMode _mode;

        public ShoulderCameraPresenter(IShoulderCameraModel model, ILookSource look)
        {
            _model = model ?? throw new System.ArgumentNullException(nameof(model));
            _look = look ?? throw new System.ArgumentNullException(nameof(look));
        }

        public CameraFrame Tick(bool toggle)
        {
            if (toggle)
                _mode = CameraFraming.Toggle(_mode);

            LookDelta look = _look.Read();
            return new CameraFrame(_model.AddLook(in look), CameraFraming.For(_mode));
        }
    }
}
