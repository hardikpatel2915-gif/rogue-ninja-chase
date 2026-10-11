using RogueNinjaChase.Player.Model;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueNinjaChase.Player.Input
{
    /// <summary>
    /// Owns the Player action map. Look and camera mode only enable their own actions.
    /// </summary>
    public sealed class PlayerMoveInput : MonoBehaviour, IMoveInputSource
    {
        [SerializeField] InputActionAsset _actions;

        InputActionMap _playerMap;
        InputAction _move;
        InputAction _sprint;
        InputAction _crouch;

        void Awake()
        {
            if (_actions == null)
                return;

            _playerMap = _actions.FindActionMap("Player", false);
            if (_playerMap == null)
                return;

            _move = _playerMap.FindAction("Move", false);
            _sprint = _playerMap.FindAction("Sprint", false);
            _crouch = _playerMap.FindAction("Crouch", false);
        }

        void OnEnable()
        {
            if (_playerMap != null)
                _playerMap.Enable();
        }

        void OnDisable()
        {
            if (_playerMap != null)
                _playerMap.Disable();
        }

        public MoveInput Read()
        {
            if (_move == null || _sprint == null || _crouch == null)
                return default;

            return MoveInputComposition.Create(
                _move.ReadValue<Vector2>(),
                _sprint.IsPressed(),
                _crouch.IsPressed());
        }
    }
}
