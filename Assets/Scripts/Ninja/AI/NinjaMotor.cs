using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    [RequireComponent(typeof(Rigidbody))]
    /// <summary>
    /// Kinematic move. A blocked forward feeler steers to a clear side.
    /// </summary>
    public sealed class NinjaMotor : MonoBehaviour
    {
        const int HitCapacity = 4;

        [SerializeField] float _arriveDistance = 0.4f;
        [SerializeField] float _turnSpeed = 360f;
        [SerializeField] float _feelerDistance = 0.8f;
        [SerializeField] LayerMask _blockers = ~0;

        readonly RaycastHit[] _hits = new RaycastHit[HitCapacity];
        Rigidbody _body;
        float _arriveSqr;

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _arriveSqr = _arriveDistance * _arriveDistance;
        }

        public bool MoveTowards(Vector3 destination, float speed)
        {
            Vector3 to = destination - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude <= _arriveSqr)
                return true;

            Vector3 desired = to / Mathf.Sqrt(to.sqrMagnitude);
            Vector3 direction = Steer(desired);
            Quaternion look = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                look,
                _turnSpeed * Time.fixedDeltaTime);

            Vector3 next = transform.position + direction * (speed * Time.fixedDeltaTime);
            next.y = transform.position.y;
            _body.MovePosition(next);
            return false;
        }

        Vector3 Steer(Vector3 desired)
        {
            if (!Blocked(desired))
                return desired;

            Vector3 right = new Vector3(desired.z, 0f, -desired.x);
            if (!Blocked(right))
                return right;
            if (!Blocked(-right))
                return -right;
            return desired;
        }

        bool Blocked(Vector3 direction)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            int count = Physics.RaycastNonAlloc(
                origin,
                direction,
                _hits,
                _feelerDistance,
                _blockers,
                QueryTriggerInteraction.Ignore);

            for (int i = 0; i < count; i++)
            {
                Transform hit = _hits[i].transform;
                if (hit == transform || hit.IsChildOf(transform))
                    continue;
                return true;
            }

            return false;
        }
    }
}
