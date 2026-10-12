using RogueNinjaChase.Player.Aim;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueNinjaChase.Player.View
{
    [RequireComponent(typeof(Rigidbody))]
    /// <summary>
    /// Turns the body and places the camera. Reports Escape and click, then applies the pointer lock it is given.
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

        public InputActionAsset Actions => _actions;
        public float Sensitivity => _sensitivity;
        public float MinPitch => _minPitch;
        public float MaxPitch => _maxPitch;
        public bool EscapePressed
        {
            get
            {
                Keyboard keyboard = Keyboard.current;
                return keyboard != null && keyboard.escapeKey.wasPressedThisFrame;
            }
        }

        public bool ClickPressed
        {
            get
            {
                Mouse mouse = Mouse.current;
                return mouse != null && mouse.leftButton.wasPressedThisFrame;
            }
        }

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            EnsureRig();
        }

        public void Apply(CameraPose pose, CameraFraming framing)
        {
            _body.MoveRotation(Quaternion.Euler(0f, pose.YawDegrees, 0f));
            _pitchPivot.localRotation = Quaternion.Euler(pose.PitchDegrees, 0f, 0f);
            _pitchPivot.localPosition = new Vector3(0f, framing.PivotHeight, 0f);
            _camera.localPosition = new Vector3(framing.Side, framing.Height, -framing.Distance);
            _camera.localRotation = Quaternion.Euler(framing.LookDownDegrees, 0f, 0f);
        }

        public void ApplyCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
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
    }
}
