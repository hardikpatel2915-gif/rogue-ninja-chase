using System;
using UnityEngine;

namespace RogueNinjaChase.Player.Model
{
    public sealed class PlayerMotorModel : IPlayerMotorModel
    {
        const float DirectionEpsilonSqr = 0.0001f; // (0.01 * 0.01)

        readonly float _walkSpeed;
        readonly float _sprintSpeed;
        readonly float _crouchSpeed;

        public PlayerMotorModel(float walkSpeed, float sprintSpeed, float crouchSpeed)
        {
            if (walkSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(walkSpeed), "Speeds cannot be negative.");
            if (sprintSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(sprintSpeed), "Speeds cannot be negative.");
            if (crouchSpeed < 0f)
                throw new ArgumentOutOfRangeException(nameof(crouchSpeed), "Speeds cannot be negative.");

            _walkSpeed = walkSpeed;
            _sprintSpeed = sprintSpeed;
            _crouchSpeed = crouchSpeed;
        }

        public MotorOutput Evaluate(in MoveInput input, Vector3 cameraForward, Vector3 cameraRight)
        {
            Vector3 forward = PlanarUnit(cameraForward);
            Vector3 right = PlanarUnit(cameraRight);
            Vector3 wish = right * input.Strafe + forward * input.Forward;

            float wishSqr = wish.sqrMagnitude;
            if (wishSqr > 1f)
                wish /= Mathf.Sqrt(wishSqr);

            bool moving = wishSqr > DirectionEpsilonSqr;
            float speed = 0f;
            if (moving)
                speed = input.Crouch ? _crouchSpeed : input.Sprint ? _sprintSpeed : _walkSpeed;

            return new MotorOutput(wish * speed, input.Crouch);
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
