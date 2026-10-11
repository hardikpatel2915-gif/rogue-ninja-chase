using UnityEngine;

namespace RogueNinjaChase.Mission
{
    [RequireComponent(typeof(Collider))]
    public sealed class ExtractionZone : MonoBehaviour
    {
        void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player") || MissionFlow.Instance == null)
                return;

            MissionFlow.Instance.TryExtract();
        }
    }
}
