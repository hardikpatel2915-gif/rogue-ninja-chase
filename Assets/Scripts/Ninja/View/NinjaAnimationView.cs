using RogueNinjaChase.Ninja.AI;
using UnityEngine;

namespace RogueNinjaChase.Ninja.View
{
    /// <summary>
    /// Plays the clip it is given. Root motion stays off.
    /// </summary>
    public sealed class NinjaAnimationView : MonoBehaviour, INinjaAnimationView
    {
        const float Fade = 0.2f;

        static readonly int Patrol = Animator.StringToHash("Patrol");
        static readonly int Investigate = Animator.StringToHash("Investigate");
        static readonly int Chase = Animator.StringToHash("Chase");

        [SerializeField] RuntimeAnimatorController _controller;

        Animator _animator;
        int _playing;

        void Awake()
        {
            _animator = GetComponentInChildren<Animator>(true);
            Transform model = transform.Find("Model");
            if (_animator == null && model != null)
                _animator = model.gameObject.AddComponent<Animator>();
            if (_animator == null)
                return;

            _animator.applyRootMotion = false;
            if (_controller != null)
                _animator.runtimeAnimatorController = _controller;
            _playing = Patrol;
        }

        public void Play(NinjaClip clip)
        {
            if (_animator == null)
                return;

            int hash = Hash(clip);
            if (hash == _playing)
                return;

            _playing = hash;
            // Seconds, not a fraction of the clip, so a long patrol does not delay the next pose.
            _animator.CrossFadeInFixedTime(hash, Fade, 0, 0f);
        }

        static int Hash(NinjaClip clip)
        {
            switch (clip)
            {
                case NinjaClip.Investigate:
                    return Investigate;
                case NinjaClip.Chase:
                    return Chase;
                default:
                    return Patrol;
            }
        }
    }
}
