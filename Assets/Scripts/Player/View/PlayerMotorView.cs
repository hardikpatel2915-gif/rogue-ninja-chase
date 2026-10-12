using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Model;
using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    /// <summary>
    /// Body sensor and writer. Reports aim, stance, and ground contact, then applies the pose it is given.
    /// </summary>
    public sealed class PlayerMotorView : MonoBehaviour, IPlayerMotorView, IAimSource, IPlayerStance, ILocomotionBody
    {
        const int GroundHitCapacity = 4;

        [SerializeField] float _walkSpeed = 0.3f;
        [SerializeField] float _sprintSpeed = 1.2f;
        [SerializeField] float _crouchSpeed = 0.45f;
        [SerializeField] float _jumpSpeed = 3f;
        [SerializeField] Transform _aim;

        readonly RaycastHit[] _groundHits = new RaycastHit[GroundHitCapacity];
        Rigidbody _body;
        CapsuleCollider _capsule;
        bool _crouching;
        bool _jumping;

        public float WalkSpeed => _walkSpeed;
        public float SprintSpeed => _sprintSpeed;
        public float CrouchSpeed => _crouchSpeed;
        public float JumpSpeed => _jumpSpeed;
        public float StandingHeight { get; private set; }
        public float StandingRadius { get; private set; }
        public float StandingCenterY { get; private set; }
        public float VerticalSpeed => _body.linearVelocity.y;
        public Vector3 PlanarVelocity { get; private set; }
        public bool IsJumping => _jumping;
        public bool IsCrouching => _crouching;

        public Vector3 Forward => Aim.forward;
        public Vector3 Right => Aim.right;

        public void SetAim(Transform aim)
        {
            _aim = aim;
        }

        Transform Aim => _aim != null ? _aim : transform;

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _capsule = GetComponent<CapsuleCollider>();
            // Pitch and roll stay frozen. Yaw is free so mouse look can turn the body.
            _body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            // Collision must not store a yaw spin. The camera sets yaw, and a leftover spin fights that forever.
            _body.maxAngularVelocity = 0f;

            StandingHeight = _capsule.height;
            StandingCenterY = _capsule.center.y;
            StandingRadius = _capsule.radius;
        }

        public void SetPlanarVelocity(Vector3 planarVelocity)
        {
            PlanarVelocity = planarVelocity;
            Vector3 velocity = _body.linearVelocity;
            velocity.x = planarVelocity.x;
            velocity.z = planarVelocity.z;
            // Leave Y so gravity is not replaced every physics step.
            _body.linearVelocity = velocity;
        }

        public void ApplyCapsule(in CapsulePose pose)
        {
            if (pose.IsCrouching == _crouching)
                return;

            _crouching = pose.IsCrouching;
            // The capsule cannot be shorter than its width, so the radius drops before the height.
            if (pose.IsCrouching)
            {
                _capsule.radius = pose.Radius;
                _capsule.height = pose.Height;
            }
            else
            {
                _capsule.height = pose.Height;
                _capsule.radius = pose.Radius;
            }

            Vector3 center = _capsule.center;
            center.y = pose.CenterY;
            _capsule.center = center;
        }

        public void ApplyJump(in JumpCommand command)
        {
            if (command.SetVerticalSpeed)
            {
                Vector3 velocity = _body.linearVelocity;
                velocity.y = command.VerticalSpeed;
                _body.linearVelocity = velocity;
            }

            _jumping = command.IsJumping;
        }

        public bool ProbeGround()
        {
            // A reading for the presenter. The model decides whether this contact starts or ends a jump.
            Vector3 center = transform.TransformPoint(_capsule.center);
            float feet = center.y - _capsule.height * 0.5f + 0.05f;
            var origin = new Vector3(center.x, feet, center.z);
            int count = Physics.RaycastNonAlloc(
                origin,
                Vector3.down,
                _groundHits,
                0.2f,
                Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                Transform hit = _groundHits[i].transform;
                if (hit == transform || hit.IsChildOf(transform))
                    continue;
                return true;
            }

            return false;
        }
    }
}
