using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    [RequireComponent(typeof(Rigidbody))]
    /// <summary>
    /// Kinematic move. A blocked line to the goal keeps one side until that line is clear again.
    /// </summary>
    public sealed class NinjaMotor : MonoBehaviour, INinjaMotor
    {
        const int HitCapacity = 4;
        const float Skin = 0.04f;

        [SerializeField] float _arriveDistance = 0.4f;
        [SerializeField] float _turnSpeed = 360f;
        [SerializeField] float _probeDistance = 2.8f;
        [SerializeField] float _openDistance = 1.5f;
        [SerializeField] LayerMask _blockers = ~0;
        [SerializeField] string _modelName = "Model";

        readonly RaycastHit[] _hits = new RaycastHit[HitCapacity];
        readonly float[] _clearance = new float[DetourSelect.SampleCount];
        Rigidbody _body;
        Transform _player;
        float _arriveSqr;
        float _radius = 0.14f;
        float _probeHeight = 0.28f;
        int _side;

        void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _body.isKinematic = true;
            _body.interpolation = RigidbodyInterpolation.Interpolate;
            _arriveSqr = _arriveDistance * _arriveDistance;
            EnsureCapsule();
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                _player = player.transform;
        }

        public bool MoveTowards(Vector3 destination, float speed)
        {
            Vector3 to = destination - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude <= _arriveSqr)
                return true;

            float goalDistance = Mathf.Sqrt(to.sqrMagnitude);
            Vector3 desired = to / goalDistance;
            Vector3 direction = Steer(desired, goalDistance);
            Quaternion look = Quaternion.LookRotation(direction, Vector3.up);
            // MoveRotation with MovePosition. Writing transform.rotation cancels the move while turning.
            _body.MoveRotation(Quaternion.RotateTowards(
                _body.rotation,
                look,
                _turnSpeed * Time.fixedDeltaTime));

            float step = speed * Time.fixedDeltaTime;
            float allowed = _clearance[0] - _radius - Skin;
            if (step > allowed)
                step = allowed;
            if (step > 0f)
            {
                Vector3 next = transform.position + direction * step;
                next.y = transform.position.y;
                _body.MovePosition(next);
            }

            return false;
        }

        Vector3 Steer(Vector3 desired, float goalDistance)
        {
            float needed = goalDistance < _openDistance ? goalDistance : _openDistance;
            _clearance[0] = Clearance(desired);
            if (_clearance[0] >= needed)
            {
                _side = 0;
                return desired;
            }

            for (int i = 0; i < DetourSelect.AngleCount; i++)
            {
                float angle = DetourSelect.Angles[i];
                _clearance[1 + i] = Clearance(Turn(desired, angle));
                _clearance[1 + DetourSelect.AngleCount + i] = Clearance(Turn(desired, -angle));
            }

            int index = DetourSelect.Select(_clearance, _side, needed);
            _side = DetourSelect.SideOf(index);
            _clearance[0] = _clearance[index];
            return Turn(desired, DetourSelect.Degrees(index));
        }

        Vector3 Turn(Vector3 direction, float degrees)
        {
            if (degrees == 0f)
                return direction;

            return Quaternion.AngleAxis(degrees, Vector3.up) * direction;
        }

        float Clearance(Vector3 direction)
        {
            Vector3 side = new Vector3(direction.z, 0f, -direction.x) * (_radius * 0.85f);
            float ahead = RayClearance(direction, Vector3.zero);
            float left = RayClearance(direction, side);
            float right = RayClearance(direction, -side);
            float nearest = ahead < left ? ahead : left;
            return right < nearest ? right : nearest;
        }

        float RayClearance(Vector3 direction, Vector3 sideOffset)
        {
            Vector3 origin = transform.position + Vector3.up * _probeHeight + sideOffset;
            int count = Physics.RaycastNonAlloc(
                origin,
                direction,
                _hits,
                _probeDistance,
                _blockers,
                QueryTriggerInteraction.Ignore);

            float nearest = _probeDistance;
            for (int i = 0; i < count; i++)
            {
                Transform hit = _hits[i].transform;
                if (hit == transform || hit.IsChildOf(transform))
                    continue;
                if (_player != null && (hit == _player || hit.IsChildOf(_player)))
                    continue;
                if (_hits[i].distance < nearest)
                    nearest = _hits[i].distance;
            }

            return nearest;
        }

        void EnsureCapsule()
        {
            CapsuleCollider capsule = GetComponent<CapsuleCollider>();
            if (capsule == null)
            {
                capsule = gameObject.AddComponent<CapsuleCollider>();
                FitCapsule(capsule);
            }

            _radius = capsule.radius;
            _probeHeight = capsule.center.y;
        }

        void FitCapsule(CapsuleCollider capsule)
        {
            float height = 0.5f;
            float radius = 0.14f;
            float centerY = 0.25f;
            Transform model = transform.Find(_modelName);
            if (model != null)
            {
                Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
                if (renderers.Length > 0)
                {
                    Bounds bounds = renderers[0].bounds;
                    for (int i = 1; i < renderers.Length; i++)
                        bounds.Encapsulate(renderers[i].bounds);

                    height = Mathf.Clamp(bounds.size.y, 0.3f, 0.8f);
                    radius = Mathf.Clamp(Mathf.Max(bounds.extents.x, bounds.extents.z) * 0.65f, 0.08f, height * 0.4f);
                    centerY = transform.InverseTransformPoint(bounds.center).y;
                }
            }

            capsule.direction = 1;
            capsule.height = Mathf.Max(height, radius * 2f);
            capsule.radius = radius;
            float bottom = centerY - capsule.height * 0.5f;
            if (bottom < 0f)
                centerY -= bottom;
            capsule.center = new Vector3(0f, centerY, 0f);
        }
    }
}
