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

        [SerializeField] string _patrol = "Patrol";
        [SerializeField] string _investigate = "Investigate";
        [SerializeField] string _chase = "Chase";
        [SerializeField] string _modelName = "Model";
        [SerializeField] RuntimeAnimatorController _controller;

        Animator _animator;
        int _patrolHash;
        int _investigateHash;
        int _chaseHash;
        int _playing;

        void Awake()
        {
            _animator = GetComponentInChildren<Animator>(true);
            Transform model = transform.Find(_modelName);
            _patrolHash = Animator.StringToHash(_patrol);
            _investigateHash = Animator.StringToHash(_investigate);
            _chaseHash = Animator.StringToHash(_chase);
            if (_animator == null && model != null)
                _animator = model.gameObject.AddComponent<Animator>();
            if (_animator == null)
                return;

            _animator.applyRootMotion = false;
            if (_controller != null)
                _animator.runtimeAnimatorController = _controller;
            _playing = _patrolHash;
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

        int Hash(NinjaClip clip)
        {
            switch (clip)
            {
                case NinjaClip.Investigate:
                    return _investigateHash;
                case NinjaClip.Chase:
                    return _chaseHash;
                default:
                    return _patrolHash;
            }
        }
    }
}
