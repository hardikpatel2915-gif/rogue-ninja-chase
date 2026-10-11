using RogueNinjaChase.Ninja.Vision;
using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    public sealed class NinjaAgent : MonoBehaviour
    {
        static readonly Color PatrolColor = new Color(0.15f, 0.85f, 0.25f, 0.35f);
        static readonly Color InvestigateColor = new Color(0.95f, 0.8f, 0.15f, 0.35f);
        static readonly Color ChaseColor = new Color(0.9f, 0.15f, 0.12f, 0.4f);

        [SerializeField] float _confirmTime = 0.35f;
        [SerializeField] float _loseSightTime = 2f;
        [SerializeField] float _waitTime = 2f;
        [SerializeField] float _patrolSpeed = 1.6f;
        [SerializeField] float _chaseSpeed = 3.4f;

        NinjaVision _vision;
        NinjaMotor _motor;
        PatrolRoute _route;
        VisionConeMesh _cone;
        NinjaBrainModel _brain;
        bool _arrived;
        bool _noisePending;
        Vector3 _noisePosition;

        public NinjaState State => _brain.State;

        /// <summary>
        /// One noise for the next tick. The distraction throw calls this later.
        /// </summary>
        public void Hear(Vector3 position)
        {
            _noisePending = true;
            _noisePosition = position;
        }

        void Awake()
        {
            _vision = GetComponent<NinjaVision>();
            _cone = GetComponent<VisionConeMesh>();
            _motor = GetComponent<NinjaMotor>();
            if (_motor == null)
                _motor = gameObject.AddComponent<NinjaMotor>();
            _route = GetComponent<PatrolRoute>();
            if (_route == null)
                _route = gameObject.AddComponent<PatrolRoute>();
            _route.Capture(transform.position);
            _brain = new NinjaBrainModel(_confirmTime, _loseSightTime, _waitTime, _patrolSpeed, _chaseSpeed);
        }

        void FixedUpdate()
        {
            Transform target = _vision != null ? _vision.Target : null;
            Vector3 playerPosition = target != null ? target.position : transform.position;
            bool canSee = _vision != null && _vision.CanSeePlayer;
            var sense = new NinjaSense(canSee, playerPosition, _noisePending, _noisePosition, _arrived);
            _noisePending = false;

            NinjaOrder order = _brain.Tick(Time.fixedDeltaTime, in sense);
            _arrived = Move(order, playerPosition);
            Tint(order.State);
        }

        bool Move(NinjaOrder order, Vector3 playerPosition)
        {
            switch (order.Destination)
            {
                case NinjaDestination.Player:
                    return _motor.MoveTowards(playerPosition, order.Speed);
                case NinjaDestination.LastKnown:
                    return _motor.MoveTowards(_brain.LastKnown, order.Speed);
                default:
                    if (!_route.HasPoints)
                        return true;
                    bool arrived = _motor.MoveTowards(_route.Current, order.Speed);
                    if (arrived)
                        _route.Advance();
                    // Not "arrived" for the brain, or a finished waypoint would end the patrol.
                    return false;
            }
        }

        void Tint(NinjaState state)
        {
            if (_cone == null)
                return;

            if (state == NinjaState.Chase)
                _cone.SetColor(ChaseColor);
            else if (state == NinjaState.Investigate)
                _cone.SetColor(InvestigateColor);
            else
                _cone.SetColor(PatrolColor);
        }
    }
}
