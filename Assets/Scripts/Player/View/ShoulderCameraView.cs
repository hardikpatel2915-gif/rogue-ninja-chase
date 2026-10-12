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
        const float ProbeRadius = 0.12f;
        const float ProbePadding = 0.04f;
        const int HitCapacity = 8;

        [SerializeField] InputActionAsset _actions;
        [SerializeField] Transform _pitchPivot;
        [SerializeField] Transform _camera;
        [SerializeField] float _sensitivity = 0.12f;
        [SerializeField] float _minPitch = -35f;
        [SerializeField] float _maxPitch = 55f;

        readonly RaycastHit[] _hits = new RaycastHit[HitCapacity];
        Rigidbody _body;
        CapsuleCollider _capsule;

        public Transform CameraTransform => _camera;
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
            _capsule = GetComponent<CapsuleCollider>();
            EnsureRig();
        }

        public void ApplyBodyYaw(float yawDegrees)
        {
            _body.angularVelocity = Vector3.zero;
            _body.MoveRotation(Quaternion.Euler(0f, yawDegrees, 0f));
        }

        public void ApplyView(CameraPose pose, CameraFraming framing, bool faceMovement)
        {
            float pivotYaw = 0f;
            if (faceMovement)
            {
                Vector3 flatForward = _body.transform.forward;
                flatForward.y = 0f;
                float bodyYaw = Mathf.Atan2(flatForward.x, flatForward.z) * Mathf.Rad2Deg;
                pivotYaw = Mathf.DeltaAngle(bodyYaw, pose.YawDegrees);
            }

            // Yaw first, then pitch. Euler(pitch, yaw, 0) mixes them and the far camera shakes while the body turns.
            _pitchPivot.localRotation =
                Quaternion.AngleAxis(pivotYaw, Vector3.up) *
                Quaternion.AngleAxis(pose.PitchDegrees, Vector3.right);
            _pitchPivot.localPosition = new Vector3(0f, framing.PivotHeight, 0f);
            _camera.localRotation = Quaternion.Euler(framing.LookDownDegrees, 0f, 0f);
            _camera.localPosition = new Vector3(framing.Side, framing.Height, -framing.Distance);
            PullOutOfWalls();
        }

        void PullOutOfWalls()
        {
            Vector3 pivot = _pitchPivot.position;
            Vector3 desired = _camera.position;
            Vector3 offset = desired - pivot;
            float distance = offset.magnitude;
            if (distance < 0.001f)
                return;

            Vector3 direction = offset / distance;
            float bodyRadius = _capsule != null ? _capsule.radius : 0.35f;
            float castStart = bodyRadius + ProbeRadius + ProbePadding;
            if (castStart >= distance)
                return;

            int count = Physics.SphereCastNonAlloc(
                pivot + direction * castStart,
                ProbeRadius,
                direction,
                _hits,
                distance - castStart,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            float nearest = float.MaxValue;
            for (int i = 0; i < count; i++)
            {
                Transform hit = _hits[i].transform;
                if (hit == transform || hit.IsChildOf(transform))
                    continue;
                if (_hits[i].distance < nearest)
                    nearest = _hits[i].distance;
            }

            if (nearest == float.MaxValue)
                return;

            float pulled = castStart + nearest;
            if (pulled >= distance)
                return;

            _camera.position = pivot + direction * Mathf.Max(ProbeRadius, pulled);
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
                _pitchPivot = pivotObject.transform;
            }

            // Stay on the player root. The run correction moves the model, and the camera must not follow it.
            if (_pitchPivot.parent != transform)
                _pitchPivot.SetParent(transform, false);

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
