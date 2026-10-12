using RogueNinjaChase.Mission;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueNinjaChase.Player.Input
{
    /// <summary>
    /// Enables only Interact, so turning it off does not stop movement.
    /// </summary>
    public sealed class PlayerInteractInput : MonoBehaviour
    {
        const int HitCapacity = 32;

        [SerializeField] InputActionAsset _actions;
        [SerializeField] string _mapName = "Player";
        [SerializeField] string _actionName = "Interact";
        [SerializeField] float _reach = 1.5f;

        readonly Collider[] _hits = new Collider[HitCapacity];
        InputAction _interact;

        public void SetActions(InputActionAsset actions)
        {
            if (_actions == null)
                _actions = actions;
            Resolve();
            if (isActiveAndEnabled && _interact != null)
                _interact.Enable();
        }

        void Awake()
        {
            Resolve();
        }

        void OnEnable()
        {
            if (_interact != null)
                _interact.Enable();
        }

        void OnDisable()
        {
            if (_interact != null)
                _interact.Disable();
        }

        void Update()
        {
            if (_interact == null || !_interact.WasPressedThisFrame())
                return;

            // The overlap runs on the press, not every frame.

            Vector3 origin = transform.position + Vector3.up * 0.4f;
            int count = Physics.OverlapSphereNonAlloc(
                origin,
                _reach,
                _hits,
                ~0,
                QueryTriggerInteraction.Collide);

            for (int i = 0; i < count; i++)
            {
                IInteractable interactable = _hits[i].GetComponentInParent<IInteractable>();
                if (interactable != null && interactable.TryInteract())
                    return;
            }
        }

        void Resolve()
        {
            if (_interact != null || _actions == null)
                return;

            InputActionMap map = _actions.FindActionMap("Player", false);
            if (map == null)
                return;

            _interact = map.FindAction("Interact", false);
        }
    }
}
