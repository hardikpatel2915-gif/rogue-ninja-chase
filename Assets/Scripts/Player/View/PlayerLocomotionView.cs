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

        static readonly int Idle = Animator.StringToHash("Idle");
        static readonly int Walk = Animator.StringToHash("Walk");
        static readonly int Sprint = Animator.StringToHash("Sprint");
        static readonly int CrouchIdle = Animator.StringToHash("CrouchIdle");
        static readonly int CrouchWalk = Animator.StringToHash("CrouchWalk");
        static readonly int Jump = Animator.StringToHash("Jump");
        static readonly int Throw = Animator.StringToHash("Throw");
        static readonly int StrafeLeft = Animator.StringToHash("StrafeLeft");
        static readonly int StrafeRight = Animator.StringToHash("StrafeRight");
        static readonly int StrafeLeftSprint = Animator.StringToHash("StrafeLeftSprint");
        static readonly int StrafeRightSprint = Animator.StringToHash("StrafeRightSprint");

        [SerializeField] RuntimeAnimatorController _controller;

        Animator _animator;

        public bool ThrowInTransition => _animator != null && _animator.IsInTransition(0);

        public bool InThrowState =>
            _animator != null
            && !_animator.IsInTransition(0)
            && _animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Throw;

        public float ThrowNormalizedTime =>
            InThrowState ? _animator.GetCurrentAnimatorStateInfo(0).normalizedTime : 0f;

        void Awake()
        {
            _animator = GetComponentInChildren<Animator>(true);
            Transform model = transform.Find("Model");
            if (_animator == null && model != null)
                _animator = model.gameObject.AddComponent<Animator>();
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

        static int Hash(LocomotionClip clip)
        {
            switch (clip)
            {
                case LocomotionClip.Walk:
                    return Walk;
                case LocomotionClip.Sprint:
                    return Sprint;
                case LocomotionClip.CrouchIdle:
                    return CrouchIdle;
                case LocomotionClip.CrouchWalk:
                    return CrouchWalk;
                case LocomotionClip.Jump:
                    return Jump;
                case LocomotionClip.Throw:
                    return Throw;
                case LocomotionClip.StrafeLeft:
                    return StrafeLeft;
                case LocomotionClip.StrafeRight:
                    return StrafeRight;
                case LocomotionClip.StrafeLeftSprint:
                    return StrafeLeftSprint;
                case LocomotionClip.StrafeRightSprint:
                    return StrafeRightSprint;
                default:
                    return Idle;
            }
        }
    }
}
