using UnityEngine;

namespace RogueNinjaChase.Mission
{
    [RequireComponent(typeof(Collider))]
    /// <summary>
    /// E works while the player is close. The chest opens and stays.
    /// </summary>
    public sealed class DossierPickup : MonoBehaviour, IInteractable
    {
        const float OpenSeconds = 1f;

        [SerializeField] MissionFlow _mission;
        [SerializeField] string _playerTag = "Player";
        [SerializeField] string _chestName = "Chest";
        [SerializeField] string _openState = "Open";

        [SerializeField] RuntimeAnimatorController _openController;

        Animator _animator;
        int _openHash;
        float _openSpeed = 4f;
        float _openTimer;
        bool _opening;
        bool _opened;
        bool _playerInside;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_playerTag))
                _playerInside = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(_playerTag))
                _playerInside = false;
        }

        void Awake()
        {
            if (_mission == null)
                _mission = FindAnyObjectByType<MissionFlow>();
            _openHash = Animator.StringToHash(_openState);
            Transform chest = transform.Find(_chestName);
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
            if (_mission != null)
                _mission.TryPickup();
        }

        public bool TryInteract()
        {
            if (!_playerInside || _opened || _mission == null || _mission.Phase != MissionPhase.Infiltrate)
                return false;

            _opened = true;
            _opening = true;
            _openTimer = 0f;
            if (_animator != null)
            {
                _animator.speed = _openSpeed;
                _animator.Play(_openHash, 0, 0f);
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
