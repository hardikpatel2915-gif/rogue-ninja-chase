using System;
using UnityEngine;

namespace RogueNinjaChase.Player.Model
{
    /// <summary>
    /// Camera-relative planar velocity. Crouch wins over sprint and blocks jump. No physics here.
    /// </summary>
    public sealed class PlayerMotorModel : IPlayerMotorModel
    {
        const float DirectionEpsilonSqr = 0.0001f; // (0.01 * 0.01)
        const float LandVelocity = 0.1f;

        readonly float _walkSpeed;
        readonly float _sprintSpeed;
        readonly float _crouchSpeed;
        readonly float _jumpSpeed;
        readonly float _jumpLatchMax;
        bool _jumpLatched;
        bool _leftGround;
        float _jumpTime;

        public PlayerMotorModel(
            float walkSpeed,
            float sprintSpeed,
            float crouchSpeed,
            float jumpSpeed = 3f,
            float jumpLatchMax = 0.8f)
        {
            if (walkSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(walkSpeed), "Speeds cannot be negative.");
            if (sprintSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(sprintSpeed), "Speeds cannot be negative.");
            if (crouchSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(crouchSpeed), "Speeds cannot be negative.");
            if (jumpSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(jumpSpeed), "Speeds cannot be negative.");
            if (jumpLatchMax < 0f)
                throw new ArgumentOutOfRangeException(nameof(jumpLatchMax), "Jump latch cannot be negative.");

            _walkSpeed = walkSpeed;
            _sprintSpeed = sprintSpeed;
            _crouchSpeed = crouchSpeed;
            _jumpSpeed = jumpSpeed;
            _jumpLatchMax = jumpLatchMax;
        }

        public MotorOutput Evaluate(in MoveInput input, Vector3 cameraForward, Vector3 cameraRight)
        {
            Vector3 forward = PlanarUnit(cameraForward);
            Vector3 right = PlanarUnit(cameraRight);
            Vector3 wish = right * input.Strafe + forward * input.Forward;

            float wishSqr = wish.sqrMagnitude;
            // A diagonal stick must not move faster than walking straight.
            if (wishSqr > 1f)
                wish /= Mathf.Sqrt(wishSqr);

            bool moving = wishSqr > DirectionEpsilonSqr;
            float speed = 0f;
            if (moving)
                speed = input.Crouch ? _crouchSpeed : input.Sprint ? _sprintSpeed : _walkSpeed;

            return new MotorOutput(wish * speed, input.Crouch, input.Jump && !input.Crouch);
        }

        /// <summary>
        /// One physics step. The latch stays up through takeoff, then clears after landing or the max air time.
        /// </summary>
        public JumpCommand AdvanceJump(bool requested, bool grounded, float verticalSpeed, float deltaTime)
        {
            bool set = false;
            if (requested && !_jumpLatched && grounded)
            {
                set = true;
                verticalSpeed = _jumpSpeed;
                _jumpLatched = true;
                _leftGround = false;
                _jumpTime = 0f;
            }

            if (_jumpLatched)
            {
                _jumpTime += deltaTime;
                if (!grounded)
                    _leftGround = true;
                else if (_leftGround && verticalSpeed <= LandVelocity)
                    _jumpLatched = false;

                if (_jumpLatched && _leftGround && _jumpTime >= _jumpLatchMax)
                    _jumpLatched = false;
            }

            return new JumpCommand(set, _jumpSpeed, _jumpLatched);
        }

        static Vector3 PlanarUnit(Vector3 value)
        {
            value.y = 0f;
            float sqr = value.sqrMagnitude;
            if (sqr < DirectionEpsilonSqr)
                return Vector3.zero;

            return value / Mathf.Sqrt(sqr);
        }
    }
}
