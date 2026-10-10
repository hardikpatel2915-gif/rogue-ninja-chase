using System;
using RogueNinjaChase.Player.Input;
using UnityEngine;

namespace RogueNinjaChase.Player.Aim
{
    public sealed class ShoulderCameraModel : IShoulderCameraModel
    {
        readonly float _sensitivity;
        readonly float _minPitch;
        readonly float _maxPitch;
        float _yaw;
        float _pitch;

        public ShoulderCameraModel(float sensitivity, float minPitch, float maxPitch, float yawDegrees, float pitchDegrees)
        {
            if (minPitch > maxPitch)
                throw new ArgumentOutOfRangeException(nameof(minPitch), "Minimum pitch cannot exceed maximum pitch.");

            _sensitivity = sensitivity;
            _minPitch = minPitch;
            _maxPitch = maxPitch;
            _yaw = WrapYaw(yawDegrees);
            _pitch = Mathf.Clamp(pitchDegrees, minPitch, maxPitch);
        }

        public CameraPose Pose => new CameraPose(_yaw, _pitch);

        public CameraPose AddLook(in LookDelta look)
        {
            _yaw = WrapYaw(_yaw + look.X * _sensitivity);
            _pitch = Mathf.Clamp(_pitch - look.Y * _sensitivity, _minPitch, _maxPitch);
            return Pose;
        }

        static float WrapYaw(float degrees)
        {
            return Mathf.Repeat(degrees + 180f, 360f) - 180f;
        }
    }
}
