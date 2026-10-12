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
        [SerializeField] string _mapName = "Player";
        [SerializeField] string _moveName = "Move";
        [SerializeField] string _sprintName = "Sprint";
        [SerializeField] string _crouchName = "Crouch";
        [SerializeField] string _jumpName = "Jump";

        InputActionMap _playerMap;
        InputAction _move;
        InputAction _sprint;
        InputAction _crouch;
        InputAction _jump;
        bool _jumpPressed;

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
            _jump = _playerMap.FindAction("Jump", false);
        }

        void Update()
        {
            // FixedUpdate can miss a tap, so the press waits until the motor reads it.
            if (_jump != null && _jump.WasPressedThisFrame())
                _jumpPressed = true;
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

            bool jump = _jumpPressed;
            _jumpPressed = false;
            return MoveInputComposition.Create(
                _move.ReadValue<Vector2>(),
                _sprint.IsPressed(),
                _crouch.IsPressed(),
                jump);
        }
    }
}
