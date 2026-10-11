using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.Model;
using RogueNinjaChase.Player.Presenter;
using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    /// <summary>
    /// Writes planar velocity and keeps gravity. Yaw stays free for the camera.
    /// </summary>
    public sealed class PlayerMotorView : MonoBehaviour, IPlayerMotorView, IAimSource, IPlayerStance
    {
        const float CrouchHeightScale = 0.5f;

        [SerializeField] float _walkSpeed = 3.5f;
        [SerializeField] float _sprintSpeed = 6f;
        [SerializeField] float _crouchSpeed = 1.75f;
        [SerializeField] Transform _aim;

        Rigidbody _body;
        CapsuleCollider _capsule;
        PlayerMotorPresenter _presenter;
        float _standingHeight;
        float _standingCenterY;
        bool _crouching;

        public bool IsCrouching => _crouching;

        public Vector3 Forward => Aim.forward;
        public Vector3 Right => Aim.right;

        Transform Aim => _aim != null ? _aim : transform;

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            // Pitch and roll stay frozen. Yaw is free so mouse look can turn the body.
            _body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _body.interpolation = RigidbodyInterpolation.Interpolate;

            _standingHeight = _capsule.height;
            _standingCenterY = _capsule.center.y;

            IMoveInputSource input = GetComponent<IMoveInputSource>();
            if (input == null)
                input = IdleMoveInputSource.Instance;

            var model = new PlayerMotorModel(_walkSpeed, _sprintSpeed, _crouchSpeed);
            _presenter = new PlayerMotorPresenter(model, this, input, this);
        }

        void FixedUpdate()
        {
            _presenter.Tick();
        }

        public void SetPlanarVelocity(Vector3 planarVelocity)
        {
            Vector3 velocity = _body.linearVelocity;
            velocity.x = planarVelocity.x;
            velocity.z = planarVelocity.z;
            // Leave Y so gravity is not replaced every physics step.
            _body.linearVelocity = velocity;
        }

        public void SetCrouching(bool crouching)
        {
            if (crouching == _crouching)
                return;

            _crouching = crouching;
            float height = crouching ? _standingHeight * CrouchHeightScale : _standingHeight;
            _capsule.height = height;

            Vector3 center = _capsule.center;
            // Drop the center by half the lost height so the feet stay on the floor.
            center.y = crouching
                ? _standingCenterY - (_standingHeight - height) * 0.5f
                : _standingCenterY;
            _capsule.center = center;
        }
    }
}
