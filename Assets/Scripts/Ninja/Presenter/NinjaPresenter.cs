using RogueNinjaChase.Ninja.AI;
using RogueNinjaChase.Ninja.View;
using UnityEngine;

namespace RogueNinjaChase.Ninja.Presenter
{
    /// <summary>
    /// Ticks the brain, moves, and chooses the clip and cone color.
    /// </summary>
    public sealed class NinjaPresenter
    {
        static readonly Color PatrolColor = new Color(0.15f, 0.85f, 0.25f, 0.35f);
        static readonly Color InvestigateColor = new Color(0.95f, 0.8f, 0.15f, 0.35f);
        static readonly Color ChaseColor = new Color(0.9f, 0.15f, 0.12f, 0.4f);

        readonly NinjaBrainModel _brain;
        readonly INinjaMotor _motor;
        bool _arrived;
        bool _noisePending;
        Vector3 _noisePosition;

        public NinjaPresenter(NinjaBrainModel brain, INinjaMotor motor)
        {
            _brain = brain;
            _motor = motor;
        }

        public NinjaState State => _brain.State;
        public bool IsMoving { get; private set; }

        public void Hear(Vector3 position)
        {
            _noisePending = true;
            _noisePosition = position;
        }

        public NinjaStep Tick(
            bool canSee,
            Vector3 playerPosition,
            Vector3 selfPosition,
            bool hasRoute,
            Vector3 routePoint,
            float deltaTime,
            float catchDistanceSqr)
        {
            var sense = new NinjaSense(canSee, playerPosition, _noisePending, _noisePosition, _arrived);
            _noisePending = false;

            NinjaOrder order = _brain.Tick(deltaTime, in sense);
            bool advanceRoute;
            _arrived = Move(order, playerPosition, hasRoute, routePoint, out advanceRoute);
            NinjaClip clip = NinjaClipSelect.Select(order.State, IsMoving);
            bool catchPlayer = ShouldCatch(order.State, selfPosition, playerPosition, catchDistanceSqr);
            return new NinjaStep(order.State, clip, advanceRoute, catchPlayer, ConeColor(order.State));
        }

        bool Move(NinjaOrder order, Vector3 playerPosition, bool hasRoute, Vector3 routePoint, out bool advanceRoute)
        {
            advanceRoute = false;
            switch (order.Destination)
            {
                case NinjaDestination.Player:
                    return Arrive(playerPosition, order.Speed);
                case NinjaDestination.LastKnown:
                    return Arrive(_brain.LastKnown, order.Speed);
                default:
                    if (!hasRoute)
                    {
                        IsMoving = false;
                        return true;
                    }

                    bool arrived = Arrive(routePoint, order.Speed);
                    if (arrived)
                        advanceRoute = true;
                    // Not arrived for the brain, or a finished waypoint would end the patrol.
                    return false;
            }
        }

        bool Arrive(Vector3 destination, float speed)
        {
            bool arrived = _motor.MoveTowards(destination, speed);
            IsMoving = !arrived;
            return arrived;
        }

        public static bool ShouldCatch(NinjaState state, Vector3 selfPosition, Vector3 playerPosition, float catchDistanceSqr)
        {
            if (state != NinjaState.Chase)
                return false;

            Vector3 delta = playerPosition - selfPosition;
            delta.y = 0f;
            return delta.sqrMagnitude <= catchDistanceSqr;
        }

        static Color ConeColor(NinjaState state)
        {
            if (state == NinjaState.Chase)
                return ChaseColor;
            if (state == NinjaState.Investigate)
                return InvestigateColor;
            return PatrolColor;
        }
    }
}
