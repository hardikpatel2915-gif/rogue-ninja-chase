using UnityEngine;

namespace RogueNinjaChase.Mission
{
    [RequireComponent(typeof(Collider))]
    public sealed class ExtractionZone : MonoBehaviour
    {
        [SerializeField] MissionFlow _mission;
        [SerializeField] string _playerTag = "Player";

        void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void Awake()
        {
            if (_mission == null)
                _mission = FindAnyObjectByType<MissionFlow>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(_playerTag) || _mission == null)
                return;

            _mission.TryExtract();
        }
    }
}
