using RogueNinjaChase.Player.View;
using UnityEngine;

namespace RogueNinjaChase.Ninja.Vision
{
    /// <summary>
    /// Sees the player inside the cone, in range, and not behind a wall.
    /// Crouching shortens the range.
    /// </summary>
    public sealed class NinjaVision : MonoBehaviour
    {
        const int HitCapacity = 16;

        [SerializeField] Transform _eye;
        [SerializeField] Transform _target;
        [SerializeField] VisionConeMesh _cone;
        [SerializeField] float _viewAngle = 90f;
        [SerializeField] float _viewDistance = 8f;
        [SerializeField] float _crouchDistanceScale = 0.5f;
        [SerializeField] float _eyeHeight = 0.45f;
        [SerializeField] float _targetHeight = 0.4f;
        [SerializeField] LayerMask _occlusion = ~0;
        [SerializeField] string _modelName = "Model";

        readonly RaycastHit[] _hits = new RaycastHit[HitCapacity];
        IPlayerStance _stance;
        float _cosHalfAngle;
        float _cachedAngle = float.NaN;

        public bool CanSeePlayer { get; private set; }
        public Transform Target => _target;

        void Awake()
        {
            if (_cone == null)
                _cone = GetComponent<VisionConeMesh>();

            if (_target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                    _target = player.transform;
            }

            if (_target != null)
                _stance = _target.GetComponentInParent<IPlayerStance>();

            // A ray above the bodies misses the obstacle standing between them.
            FitSampleHeights();
        }

        void FitSampleHeights()
        {
            if (_eye == null && TryBodyHeight(transform.Find(_modelName), out float ninjaHeight))
                _eyeHeight = ninjaHeight * 0.85f;

            if (_target == null)
                return;

            CapsuleCollider body = _target.GetComponent<CapsuleCollider>();
            if (body != null)
                _targetHeight = body.center.y;
        }

        static bool TryBodyHeight(Transform model, out float height)
        {
            height = 0f;
            if (model == null)
                return false;

            Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
                return false;

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);

            height = bounds.size.y;
            return height > 0.05f;
        }

        void LateUpdate()
        {
            if (_cachedAngle != _viewAngle)
            {
                _cachedAngle = _viewAngle;
                _cosHalfAngle = VisionMath.CosHalfAngle(_viewAngle);
            }

            Vector3 forward = PlanarForward(transform.forward);
            float range = _viewDistance;
            bool crouching = _stance != null && _stance.IsCrouching;
            if (crouching)
                range *= _crouchDistanceScale;

            CanSeePlayer = CanSee(forward, range);
            if (_cone != null)
                _cone.Show(transform.position, forward, range, _viewAngle, _occlusion);
        }

        bool CanSee(Vector3 forward, float range)
        {
            if (_target == null || forward.sqrMagnitude < 1e-8f)
                return false;

            Vector3 eye = _eye != null ? _eye.position : transform.position + Vector3.up * _eyeHeight;
            float height = _stance != null && _stance.IsCrouching ? _targetHeight * 0.5f : _targetHeight;
            Vector3 target = _target.position + Vector3.up * height;
            ConeTest test = VisionMath.TestCone(eye, forward, target, _cosHalfAngle, range * range);
            if (!test.Inside)
                return false;

            return !IsBlocked(eye, test.Direction, test.Distance);
        }

        bool IsBlocked(Vector3 eye, Vector3 direction, float distance)
        {
            int count = Physics.RaycastNonAlloc(
                eye,
                direction,
                _hits,
                distance,
                _occlusion,
                QueryTriggerInteraction.Ignore);

            // The ray stops at the player, so any other hit is a wall in between.
            for (int i = 0; i < count; i++)
            {
                Transform hit = _hits[i].transform;
                if (hit == transform || hit.IsChildOf(transform))
                    continue;
                if (_target != null && (hit == _target || hit.IsChildOf(_target)))
                    continue;
                return true;
            }

            return false;
        }

        static Vector3 PlanarForward(Vector3 forward)
        {
            forward.y = 0f;
            float sqr = forward.sqrMagnitude;
            if (sqr < 1e-8f)
                return Vector3.zero;
            return forward / Mathf.Sqrt(sqr);
        }
    }
}
