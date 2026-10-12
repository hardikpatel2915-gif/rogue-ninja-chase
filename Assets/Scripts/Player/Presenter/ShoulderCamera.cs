using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.View;
using UnityEngine;

namespace RogueNinjaChase.Player.Presenter
{
    /// <summary>
    /// Connects mouse look to the camera rig. The view does not choose framing.
    /// </summary>
    [RequireComponent(typeof(ShoulderCameraView))]
    [RequireComponent(typeof(Rigidbody))]
    public sealed class ShoulderCamera : MonoBehaviour
    {
        ShoulderCameraPresenter _presenter;
        ShoulderCameraView _view;
        PlayerCameraInput _toggle;
        CursorLockModel _cursor;

        void Awake()
        {
            _view = GetComponent<ShoulderCameraView>();
            Rigidbody body = GetComponent<Rigidbody>();
            _cursor = new CursorLockModel();

            PlayerLookInput lookInput = GetComponent<PlayerLookInput>();
            if (lookInput == null)
                lookInput = gameObject.AddComponent<PlayerLookInput>();
            lookInput.SetActions(_view.Actions);

            _toggle = GetComponent<PlayerCameraInput>();
            if (_toggle == null)
                _toggle = gameObject.AddComponent<PlayerCameraInput>();
            _toggle.SetActions(_view.Actions);

            float yaw = body.rotation.eulerAngles.y;
            var model = new ShoulderCameraModel(_view.Sensitivity, _view.MinPitch, _view.MaxPitch, yaw, 0f);
            _presenter = new ShoulderCameraPresenter(model, lookInput);
        }

        void OnEnable()
        {
            _cursor.Lock();
            _view.ApplyCursor(true);
        }

        void OnDisable()
        {
            _cursor.Unlock();
            _view.ApplyCursor(false);
        }

        void LateUpdate()
        {
            bool locked = _cursor.Tick(_view.EscapePressed, _view.ClickPressed);
            _view.ApplyCursor(locked);

            bool toggle = _toggle != null && _toggle.ConsumeToggle();
            CameraFrame frame = _presenter.Tick(toggle);
            _view.Apply(frame.Pose, frame.Framing);
        }
    }
}
