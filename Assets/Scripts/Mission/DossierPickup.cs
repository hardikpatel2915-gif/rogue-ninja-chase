using UnityEngine;

namespace RogueNinjaChase.Mission
{
    [RequireComponent(typeof(Collider))]
    /// <summary>
    /// E works only while the player is standing in the trigger.
    /// </summary>
    public sealed class DossierPickup : MonoBehaviour
    {
        bool _playerInside;

        void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

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

        public bool TryInteract()
        {
            if (!_playerInside || MissionFlow.Instance == null || !MissionFlow.Instance.TryPickup())
                return false;

            gameObject.SetActive(false);
            return true;
        }
    }
}
