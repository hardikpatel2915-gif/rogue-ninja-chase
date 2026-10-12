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
        ShoulderCamera _camera;

        public void NotifyThrow()
        {
            if (_presenter != null)
                _presenter.NotifyThrow();
        }

        void Awake()
        {
            _presenter = new PlayerLocomotionPresenter(
                GetComponent<PlayerLocomotionView>(),
                GetComponent<PlayerMotorView>());
            _camera = GetComponent<ShoulderCamera>();
        }

        void LateUpdate()
        {
            bool aimStrafe = _camera != null && _camera.Aiming;
            _presenter.Tick(aimStrafe);
        }
    }
}
