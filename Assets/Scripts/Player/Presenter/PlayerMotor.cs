using RogueNinjaChase.Player.Aim;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.Model;
using RogueNinjaChase.Player.View;
using UnityEngine;

namespace RogueNinjaChase.Player.Presenter
{
    /// <summary>
    /// Connects the motor model to the body. The view does not construct or tick either one.
    /// </summary>
    [RequireComponent(typeof(PlayerMotorView))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        PlayerMotorPresenter _presenter;
        PlayerLocomotion _locomotion;

        public void Stop()
        {
            if (_presenter != null)
                _presenter.Stop();
        }

        void Awake()
        {
            _locomotion = GetComponent<PlayerLocomotion>();
            PlayerMotorView view = GetComponent<PlayerMotorView>();
            IMoveInputSource input = GetComponent<IMoveInputSource>();
            if (input == null)
                input = IdleMoveInputSource.Instance;

            var model = new PlayerMotorModel(
                view.WalkSpeed,
                view.SprintSpeed,
                view.CrouchSpeed,
                view.JumpSpeed);
            _presenter = new PlayerMotorPresenter(model, view, input, view);
        }

        void FixedUpdate()
        {
            bool held = _locomotion != null && _locomotion.Throwing;
            _presenter.Tick(Time.fixedDeltaTime, held);
        }
    }
}
