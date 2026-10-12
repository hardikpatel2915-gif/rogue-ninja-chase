using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.Model;
using RogueNinjaChase.Player.View;
using UnityEngine;

namespace RogueNinjaChase.Player.Presenter
{
    /// <summary>
    /// Connects mouse look to the camera rig. The view does not choose framing.
    /// </summary>
    [RequireComponent(typeof(ShoulderCameraView))]
    [RequireComponent(typeof(Rigidbody))]
    [DefaultExecutionOrder(50)]
    public sealed class ShoulderCamera : MonoBehaviour
    {
        const float FollowTurnSpeed = 540f;

        ShoulderCameraPresenter _presenter;
        ShoulderCameraView _view;
        PlayerCameraInput _toggle;
        CursorLockModel _cursor;
        AimCrosshair _crosshair;
        CameraFrame _frame;
        Rigidbody _body;
        float _facingYaw;
        bool _ready;

        public bool Aiming => _presenter != null && CameraFraming.AllowsAim(_presenter.Mode);

        void Awake()
        {
            _view = GetComponent<ShoulderCameraView>();
            _body = GetComponent<Rigidbody>();
            _cursor = new CursorLockModel();

            PlayerLookInput lookInput = GetComponent<PlayerLookInput>();
            if (lookInput == null)
                lookInput = gameObject.AddComponent<PlayerLookInput>();
            lookInput.SetActions(_view.Actions);

            _toggle = GetComponent<PlayerCameraInput>();
            if (_toggle == null)
                _toggle = gameObject.AddComponent<PlayerCameraInput>();
            _toggle.SetActions(_view.Actions);

            float yaw = _body.rotation.eulerAngles.y;
            _facingYaw = yaw;
            var model = new ShoulderCameraModel(_view.Sensitivity, _view.MinPitch, _view.MaxPitch, yaw, 0f);
            _presenter = new ShoulderCameraPresenter(model, lookInput);
        }

        void Start()
        {
            PlayerMotorView motor = GetComponent<PlayerMotorView>();
            if (motor != null && _view.CameraTransform != null)
                motor.SetAim(_view.CameraTransform);
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

        void Update()
        {
            bool toggle = _toggle != null && _toggle.ConsumeToggle();
            _frame = _presenter.Tick(toggle);
            _ready = true;
        }

        void FixedUpdate()
        {
            if (!_ready)
                return;

            // Yaw on the physics step. Doing it after interpolation fights the body and the shake stays.
            if (Aiming)
            {
                _facingYaw = _frame.Pose.YawDegrees;
                _view.ApplyBodyYaw(_facingYaw);
                return;
            }

            Vector3 velocity = _body.linearVelocity;
            velocity.y = 0f;
            _facingYaw = MoveFacing.YawDegrees(_facingYaw, velocity, FollowTurnSpeed, Time.fixedDeltaTime);
            _view.ApplyBodyYaw(_facingYaw);
        }

        void LateUpdate()
        {
            bool locked = _cursor.Tick(_view.EscapePressed, _view.ClickPressed);
            _view.ApplyCursor(locked);
            if (!_ready)
                return;

            _view.ApplyView(_frame.Pose, _frame.Framing, !Aiming);
            if (_crosshair == null)
                _crosshair = GetComponent<AimCrosshair>();
            if (_crosshair != null)
                _crosshair.SetVisible(Aiming);
        }
    }
}
