using RogueNinjaChase.Player.View;
using UnityEngine;

namespace RogueNinjaChase.Player.Presenter
{
    /// <summary>
    /// Connects the body to the animator. The locomotion view does not choose the clip.
    /// </summary>
    [RequireComponent(typeof(PlayerLocomotionView))]
    [RequireComponent(typeof(PlayerMotorView))]
    public sealed class PlayerLocomotion : MonoBehaviour
    {
        PlayerLocomotionPresenter _presenter;
        PlayerMotor _motor;
        ShoulderCamera _camera;

        public bool Throwing => _presenter != null && _presenter.Throwing;

        public void NotifyThrow()
        {
            if (_presenter != null)
                _presenter.NotifyThrow();
            // The motor's next physics step is too late. The sprint velocity is already on the body.
            if (_motor != null)
                _motor.Stop();
        }

        void Awake()
        {
            _presenter = new PlayerLocomotionPresenter(
                GetComponent<PlayerLocomotionView>(),
                GetComponent<PlayerMotorView>());
            _motor = GetComponent<PlayerMotor>();
            _camera = GetComponent<ShoulderCamera>();
        }

        void LateUpdate()
        {
            bool aimStrafe = _camera != null && _camera.Aiming;
            _presenter.Tick(aimStrafe);
        }
    }
}
