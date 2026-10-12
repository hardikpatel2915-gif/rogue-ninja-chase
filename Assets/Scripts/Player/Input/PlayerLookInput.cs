using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueNinjaChase.Player.Input
{
    public sealed class PlayerLookInput : MonoBehaviour, ILookSource
    {
        [SerializeField] InputActionAsset _actions;
        [SerializeField] string _mapName = "Player";
        [SerializeField] string _lookName = "Look";

        InputAction _look;

        public void SetActions(InputActionAsset actions)
        {
            if (_actions == null)
                _actions = actions;

            Resolve();
            if (isActiveAndEnabled && _look != null)
                _look.Enable();
        }

        void Awake()
        {
            Resolve();
        }

        void OnEnable()
        {
            if (_look != null)
                _look.Enable();
        }

        void OnDisable()
        {
            // Disable this action only. Disabling the map would stop movement too.
            if (_look != null)
                _look.Disable();
        }

        public LookDelta Read()
        {
            if (_look == null)
                return default;

            Vector2 delta = _look.ReadValue<Vector2>();
            return new LookDelta(delta.x, delta.y);
        }

        void Resolve()
        {
            if (_look != null || _actions == null)
                return;

            InputActionMap map = _actions.FindActionMap("Player", false);
            if (map == null)
                return;

            _look = map.FindAction("Look", false);
        }
    }
}
