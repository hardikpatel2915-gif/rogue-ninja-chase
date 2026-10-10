using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.Model;
using RogueNinjaChase.Player.Presenter;
using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public sealed class PlayerMotorView : MonoBehaviour, IPlayerMotorView, IAimSource
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

        public Vector3 Forward => Aim.forward;
        public Vector3 Right => Aim.right;

        Transform Aim => _aim != null ? _aim : transform;

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            _body.constraints = RigidbodyConstraints.FreezeRotation;
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
            center.y = crouching
                ? _standingCenterY - (_standingHeight - height) * 0.5f
                : _standingCenterY;
            _capsule.center = center;
        }
    }
}
