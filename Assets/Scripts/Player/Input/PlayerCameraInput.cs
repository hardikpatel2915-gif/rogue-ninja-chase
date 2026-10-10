using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueNinjaChase.Player.Input
{
    public sealed class PlayerCameraInput : MonoBehaviour
    {
        [SerializeField] InputActionAsset _actions;

        InputAction _camera;

        public void SetActions(InputActionAsset actions)
        {
            if (_actions == null)
                _actions = actions;

            Resolve();
            if (isActiveAndEnabled && _camera != null)
                _camera.Enable();
        }

        void Awake()
        {
            Resolve();
        }

        void OnEnable()
        {
            if (_camera != null)
                _camera.Enable();
        }

        void OnDisable()
        {
            if (_camera != null)
                _camera.Disable();
        }

        public bool ConsumeToggle()
        {
            return _camera != null && _camera.WasPressedThisFrame();
        }

        void Resolve()
        {
            if (_camera != null || _actions == null)
                return;

            InputActionMap map = _actions.FindActionMap("Player", false);
            if (map == null)
                return;

            _camera = map.FindAction("Camera", false);
        }
    }
}
