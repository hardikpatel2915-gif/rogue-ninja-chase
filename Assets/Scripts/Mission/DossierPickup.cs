using UnityEngine;

namespace RogueNinjaChase.Mission
{
    [RequireComponent(typeof(Collider))]
    /// <summary>
    /// E works while the player is close. The chest opens and stays.
    /// </summary>
    public sealed class DossierPickup : MonoBehaviour
    {
        const float OpenSeconds = 1f;

        static readonly int Open = Animator.StringToHash("Open");

        [SerializeField] RuntimeAnimatorController _openController;

        Animator _animator;
        float _openSpeed = 4f;
        float _openTimer;
        bool _opening;
        bool _opened;
        bool _playerInside;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                _playerInside = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                _playerInside = false;
        }

        void Awake()
        {
            Transform chest = transform.Find("Chest");
            if (chest == null)
                return;

            _animator = chest.GetComponent<Animator>();
            if (_animator == null)
                _animator = chest.gameObject.AddComponent<Animator>();
            _animator.applyRootMotion = false;
            if (_openController != null)
                _animator.runtimeAnimatorController = _openController;
            _openSpeed = SpeedFor(_animator);
        }

        void Update()
        {
            if (!_opening)
                return;

            _openTimer += Time.deltaTime;
            if (_openTimer < OpenSeconds)
                return;

            _opening = false;
            // The carry line waits until the lid has finished.
            if (MissionFlow.Instance != null)
                MissionFlow.Instance.TryPickup();
        }

        public bool TryInteract()
        {
            if (!_playerInside || _opened || MissionFlow.Instance == null || MissionFlow.Instance.Phase != MissionPhase.Infiltrate)
                return false;

            _opened = true;
            _opening = true;
            _openTimer = 0f;
            if (_animator != null)
            {
                _animator.speed = _openSpeed;
                _animator.Play(Open, 0, 0f);
            }

            return true;
        }

        static float SpeedFor(Animator animator)
        {
            if (animator == null || animator.runtimeAnimatorController == null)
                return 4f;

            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for (int i = 0; i < clips.Length; i++)
            {
                AnimationClip clip = clips[i];
                if (clip != null && clip.length > 0.01f)
                    return clip.length / OpenSeconds;
            }

            return 4f;
        }
    }
}
