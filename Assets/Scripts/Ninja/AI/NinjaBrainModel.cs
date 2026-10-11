using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    /// <summary>
    /// Patrol, investigate, and chase. A glimpse is not a chase; the player must stay in view.
    /// </summary>
    public sealed class NinjaBrainModel
    {
        readonly float _confirmTime;
        readonly float _loseSightTime;
        readonly float _waitTime;
        readonly float _patrolSpeed;
        readonly float _chaseSpeed;

        NinjaState _state;
        float _confirm;
        float _lostSight;
        float _wait;
        Vector3 _lastKnown;

        public NinjaBrainModel(
            float confirmTime,
            float loseSightTime,
            float waitTime,
            float patrolSpeed,
            float chaseSpeed)
        {
            _confirmTime = confirmTime;
            _loseSightTime = loseSightTime;
            _waitTime = waitTime;
            _patrolSpeed = patrolSpeed;
            _chaseSpeed = chaseSpeed;
        }

        public NinjaState State => _state;
        public Vector3 LastKnown => _lastKnown;

        public NinjaOrder Tick(float deltaTime, in NinjaSense sense)
        {
            if (sense.CanSeePlayer)
            {
                _lastKnown = sense.PlayerPosition;
                _lostSight = 0f;
                _confirm += deltaTime;
            }
            else
            {
                _confirm = 0f;
            }

            if (sense.HasNoise && _state != NinjaState.Chase)
            {
                _lastKnown = sense.NoisePosition;
                _state = NinjaState.Investigate;
                _wait = 0f;
            }

            switch (_state)
            {
                case NinjaState.Patrol:
                    if (sense.CanSeePlayer && _confirm >= _confirmTime)
                        _state = NinjaState.Chase;
                    break;
                case NinjaState.Investigate:
                    if (sense.CanSeePlayer && _confirm >= _confirmTime)
                        _state = NinjaState.Chase;
                    else if (sense.AtDestination)
                    {
                        _wait += deltaTime;
                        if (_wait >= _waitTime)
                            _state = NinjaState.Patrol;
                    }
                    break;
                default:
                    if (!sense.CanSeePlayer)
                    {
                        _lostSight += deltaTime;
                        if (_lostSight >= _loseSightTime)
                        {
                            _state = NinjaState.Investigate;
                            _wait = 0f;
                        }
                    }
                    break;
            }

            return OrderFor(sense.CanSeePlayer);
        }

        NinjaOrder OrderFor(bool canSeePlayer)
        {
            switch (_state)
            {
                case NinjaState.Chase:
                    return new NinjaOrder(
                        NinjaState.Chase,
                        canSeePlayer ? NinjaDestination.Player : NinjaDestination.LastKnown,
                        _chaseSpeed);
                case NinjaState.Investigate:
                    return new NinjaOrder(NinjaState.Investigate, NinjaDestination.LastKnown, _patrolSpeed);
                default:
                    return new NinjaOrder(NinjaState.Patrol, NinjaDestination.Route, _patrolSpeed);
            }
        }
    }
}
