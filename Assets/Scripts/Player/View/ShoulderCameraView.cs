using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.Presenter;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueNinjaChase.Player.View
{
    [RequireComponent(typeof(Rigidbody))]
    /// <summary>
    /// Yaw turns the body. Pitch stays on the camera. V switches shoulder and follow framing.
    /// </summary>
    public sealed class ShoulderCameraView : MonoBehaviour
    {
        [SerializeField] InputActionAsset _actions;
        [SerializeField] Transform _pitchPivot;
        [SerializeField] Transform _camera;
        [SerializeField] float _sensitivity = 0.12f;
        [SerializeField] float _minPitch = -35f;
        [SerializeField] float _maxPitch = 55f;

        Rigidbody _body;
        ShoulderCameraPresenter _presenter;
        PlayerCameraInput _cameraInput;
        CameraViewMode _mode;
        bool _locked;

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            EnsureRig();

            PlayerLookInput lookInput = GetComponent<PlayerLookInput>();
            if (lookInput == null)
                lookInput = gameObject.AddComponent<PlayerLookInput>();
            lookInput.SetActions(_actions);
            ILookSource look = lookInput;

            _cameraInput = GetComponent<PlayerCameraInput>();
            if (_cameraInput == null)
                _cameraInput = gameObject.AddComponent<PlayerCameraInput>();
            _cameraInput.SetActions(_actions);

            float yaw = _body.rotation.eulerAngles.y;
            var model = new ShoulderCameraModel(_sensitivity, _minPitch, _maxPitch, yaw, 0f);
            _presenter = new ShoulderCameraPresenter(model, look);
        }

        void OnEnable()
        {
            LockCursor();
        }

        void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            _locked = false;
        }

        void LateUpdate()
        {
            UpdateCursor();
            if (_cameraInput.ConsumeToggle())
                _mode = CameraFraming.Toggle(_mode);

            CameraPose pose = _presenter.Tick();
            CameraFraming framing = CameraFraming.For(_mode);
            _body.MoveRotation(Quaternion.Euler(0f, pose.YawDegrees, 0f));
            _pitchPivot.localRotation = Quaternion.Euler(pose.PitchDegrees, 0f, 0f);
            _pitchPivot.localPosition = new Vector3(0f, framing.PivotHeight, 0f);
            _camera.localPosition = new Vector3(framing.Side, framing.Height, -framing.Distance);
            _camera.localRotation = Quaternion.Euler(framing.LookDownDegrees, 0f, 0f);
        }

        void EnsureRig()
        {
            if (_pitchPivot == null)
            {
                var pivotObject = new GameObject("CameraPivot");
                pivotObject.transform.SetParent(transform, false);
                _pitchPivot = pivotObject.transform;
            }

            if (_camera != null)
                return;

            var cameraObject = new GameObject("ShoulderCamera");
            cameraObject.transform.SetParent(_pitchPivot, false);
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 50f;
            if (cameraObject.GetComponent<AudioListener>() == null)
                cameraObject.AddComponent<AudioListener>();
            _camera = cameraObject.transform;
        }

        void UpdateCursor()
        {
            Keyboard keyboard = Keyboard.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _locked = false;
                return;
            }

            Mouse mouse = Mouse.current;
            if (!_locked && mouse != null && mouse.leftButton.wasPressedThisFrame)
                LockCursor();
        }

        void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _locked = true;
        }
    }
}
