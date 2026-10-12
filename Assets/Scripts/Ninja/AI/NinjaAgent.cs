using RogueNinjaChase.Core;
using RogueNinjaChase.Mission;
using RogueNinjaChase.Ninja.Presenter;
using RogueNinjaChase.Ninja.View;
using RogueNinjaChase.Ninja.Vision;
using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    /// <summary>
    /// Connects vision, the body, and the animator. The brain decides the state.
    /// </summary>
    public sealed class NinjaAgent : MonoBehaviour, INoiseListener
    {
        [SerializeField] float _confirmTime = 0.35f;
        [SerializeField] float _loseSightTime = 2f;
        [SerializeField] float _waitTime = 2f;
        [SerializeField] float _patrolSpeed = 1.1f;
        [SerializeField] float _chaseSpeed = 3.2f;
        [SerializeField] float _catchDistance = 1.25f;
        [SerializeField] MissionFlow _mission;

        NinjaVision _vision;
        NinjaMotor _motor;
        PatrolRoute _route;
        VisionConeMesh _cone;
        NinjaAnimationView _animation;
        NinjaPresenter _presenter;
        NinjaClip _clip = NinjaClip.Patrol;
        bool _clipPending;
        float _catchSqr;
        IMission _rules;

        public NinjaState State => _presenter.State;
        public Vector3 Position => transform.position;

        /// <summary>
        /// One noise for the next tick. The distraction throw calls this later.
        /// </summary>
        public void Hear(Vector3 position)
        {
            _presenter.Hear(position);
        }

        void Awake()
        {
            _vision = GetComponent<NinjaVision>();
            _cone = GetComponent<VisionConeMesh>();
            _animation = GetComponent<NinjaAnimationView>();
            _motor = GetComponent<NinjaMotor>();
            if (_motor == null)
                _motor = gameObject.AddComponent<NinjaMotor>();
            _route = GetComponent<PatrolRoute>();
            if (_route == null)
                _route = gameObject.AddComponent<PatrolRoute>();
            _route.Capture(transform.position);
            var brain = new NinjaBrainModel(_confirmTime, _loseSightTime, _waitTime, _patrolSpeed, _chaseSpeed);
            _presenter = new NinjaPresenter(brain, _motor);
            _catchSqr = _catchDistance * _catchDistance;
            if (_mission == null)
                _mission = FindAnyObjectByType<MissionFlow>();
            _rules = _mission;
        }

        void FixedUpdate()
        {
            Transform target = _vision != null ? _vision.Target : null;
            Vector3 playerPosition = target != null ? target.position : transform.position;
            bool canSee = _vision != null && _vision.CanSeePlayer;
            bool hasRoute = _route.HasPoints;
            Vector3 routePoint = hasRoute ? _route.Current : transform.position;
            NinjaStep step = _presenter.Tick(
                canSee,
                playerPosition,
                transform.position,
                hasRoute,
                routePoint,
                Time.fixedDeltaTime,
                _catchSqr);

            if (step.AdvanceRoute)
                _route.Advance();
            if (_cone != null)
                _cone.SetColor(step.ConeColor);
            if (step.Catch && target != null && _rules != null)
                _rules.TryCatch();
            if (step.Clip != _clip)
            {
                _clip = step.Clip;
                _clipPending = true;
            }
        }

        void LateUpdate()
        {
            if (!_clipPending || _animation == null)
                return;

            _clipPending = false;
            _animation.Play(_clip);
        }
    }
}
