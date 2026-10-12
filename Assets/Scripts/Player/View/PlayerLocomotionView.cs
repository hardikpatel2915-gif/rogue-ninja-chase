using RogueNinjaChase.Animation;
using RogueNinjaChase.Player.Model;
using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    /// <summary>
    /// Plays the clip it is given.
    /// </summary>
    public sealed class PlayerLocomotionView : MonoBehaviour, IPlayerLocomotionView
    {
        const float Fade = 0.12f;

        [SerializeField] string _idle = "Idle";
        [SerializeField] string _walk = "Walk";
        [SerializeField] string _sprint = "Sprint";
        [SerializeField] string _crouchIdle = "CrouchIdle";
        [SerializeField] string _crouchWalk = "CrouchWalk";
        [SerializeField] string _jump = "Jump";
        [SerializeField] string _throw = "Throw";
        [SerializeField] string _strafeLeft = "StrafeLeft";
        [SerializeField] string _strafeRight = "StrafeRight";
        [SerializeField] string _strafeLeftSprint = "StrafeLeftSprint";
        [SerializeField] string _strafeRightSprint = "StrafeRightSprint";
        [SerializeField] string _modelName = "Model";
        [SerializeField] RuntimeAnimatorController _controller;

        Animator _animator;
        int _idleHash;
        int _walkHash;
        int _sprintHash;
        int _crouchIdleHash;
        int _crouchWalkHash;
        int _jumpHash;
        int _throwHash;
        int _strafeLeftHash;
        int _strafeRightHash;
        int _strafeLeftSprintHash;
        int _strafeRightSprintHash;

        public bool ThrowInTransition => _animator != null && _animator.IsInTransition(0);

        public bool InThrowState =>
            _animator != null
            && !_animator.IsInTransition(0)
            && _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == _throwHash;

        public float ThrowNormalizedTime =>
            InThrowState ? _animator.GetCurrentAnimatorStateInfo(0).normalizedTime : 0f;

        void Awake()
        {
            _animator = GetComponentInChildren<Animator>(true);
            Transform model = transform.Find(_modelName);
            if (_animator == null && model != null)
                _animator = model.gameObject.AddComponent<Animator>();
            CacheStates();
            if (_animator == null)
                return;

            DiscardRootMotion.Attach(_animator);
            if (_controller != null)
                _animator.runtimeAnimatorController = _controller;
        }

        public void Play(LocomotionClip clip)
        {
            if (_animator == null)
                return;

            // Seconds, not a fraction of the clip. A long idle must not hold the old pose after movement starts.
            _animator.CrossFadeInFixedTime(Hash(clip), Fade, 0, 0f);
        }

        void CacheStates()
        {
            _idleHash = Animator.StringToHash(_idle);
            _walkHash = Animator.StringToHash(_walk);
            _sprintHash = Animator.StringToHash(_sprint);
            _crouchIdleHash = Animator.StringToHash(_crouchIdle);
            _crouchWalkHash = Animator.StringToHash(_crouchWalk);
            _jumpHash = Animator.StringToHash(_jump);
            _throwHash = Animator.StringToHash(_throw);
            _strafeLeftHash = Animator.StringToHash(_strafeLeft);
            _strafeRightHash = Animator.StringToHash(_strafeRight);
            _strafeLeftSprintHash = Animator.StringToHash(_strafeLeftSprint);
            _strafeRightSprintHash = Animator.StringToHash(_strafeRightSprint);
        }

        int Hash(LocomotionClip clip)
        {
            switch (clip)
            {
                case LocomotionClip.Walk:
                    return _walkHash;
                case LocomotionClip.Sprint:
                    return _sprintHash;
                case LocomotionClip.CrouchIdle:
                    return _crouchIdleHash;
                case LocomotionClip.CrouchWalk:
                    return _crouchWalkHash;
                case LocomotionClip.Jump:
                    return _jumpHash;
                case LocomotionClip.Throw:
                    return _throwHash;
                case LocomotionClip.StrafeLeft:
                    return _strafeLeftHash;
                case LocomotionClip.StrafeRight:
                    return _strafeRightHash;
                case LocomotionClip.StrafeLeftSprint:
                    return _strafeLeftSprintHash;
                case LocomotionClip.StrafeRightSprint:
                    return _strafeRightSprintHash;
                default:
                    return _idleHash;
            }
        }
    }
}
