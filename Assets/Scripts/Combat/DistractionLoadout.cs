using RogueNinjaChase.Ninja.AI;
using RogueNinjaChase.Player.View;
using RogueNinjaChase.Pool;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace RogueNinjaChase.Combat
{
    /// <summary>
    /// Q throws one smoke bomb at the crosshair. Another throw waits until that bomb is gone.
    /// </summary>
    public sealed class DistractionLoadout : MonoBehaviour
    {
        [SerializeField] InputActionAsset _actions;
        [SerializeField] Material _projectileMaterial;
        [SerializeField] Material _smokeMaterial;
        [SerializeField] int _capacity = 4;
        [SerializeField] float _speed = 14f;
        [SerializeField] float _projectileLife = 2f;
        [SerializeField] float _smokeLife = 1.6f;
        [SerializeField] float _noiseRadius = 8f;
        [SerializeField] Vector3 _handLocal = new Vector3(0.28f, 1.05f, 0.35f);
        [SerializeField] float _aimDistance = 12f;

        PrefabPool _projectiles;
        PrefabPool _flashes;
        DistractionProjectile[] _projectileScripts;
        readonly RaycastHit[] _aimHits = new RaycastHit[8];
        NinjaAgent[] _ninjas;
        InputAction _throw;
        Transform _aim;
        float _noiseSqr;
        bool _projectileInFlight;

        public void SetActions(InputActionAsset actions)
        {
            if (_actions == null)
                _actions = actions;
            Resolve();
            if (isActiveAndEnabled && _throw != null)
                _throw.Enable();
        }

        void Awake()
        {
            _noiseSqr = _noiseRadius * _noiseRadius;
            var root = new GameObject("DistractionPool").transform;
            _projectiles = CreatePool(root, _capacity, 0.12f, _projectileMaterial, true, out _projectileScripts);
            _flashes = CreatePool(root, _capacity, 0.45f, _smokeMaterial, false, out SmokeCloud[] _);
            IgnorePlayerColliders();
            if (GetComponent<AimCrosshair>() == null)
                gameObject.AddComponent<AimCrosshair>();
            Resolve();
        }

        void Start()
        {
            _ninjas = FindObjectsByType<NinjaAgent>(FindObjectsSortMode.None);
            Camera childCamera = GetComponentInChildren<Camera>();
            _aim = childCamera != null ? childCamera.transform : transform;
        }

        void OnEnable()
        {
            if (_throw != null)
                _throw.Enable();
        }

        void OnDisable()
        {
            // This action only. Disabling the map would stop movement.
            if (_throw != null)
                _throw.Disable();
        }

        void Update()
        {
            if (_projectileInFlight || _throw == null || !_throw.WasPressedThisFrame())
                return;

            Throw();
        }

        public void Impact(Vector3 position)
        {
            Vector3 ground = GroundPoint(position);
            if (_flashes.TrySpawn(ground, Quaternion.identity, out GameObject smokeObject))
            {
                SmokeCloud smoke = smokeObject.GetComponent<SmokeCloud>();
                smoke.Play(ground);
            }

            if (_ninjas == null)
                return;

            for (int i = 0; i < _ninjas.Length; i++)
            {
                Vector3 delta = position - _ninjas[i].transform.position;
                if (delta.sqrMagnitude <= _noiseSqr)
                    _ninjas[i].Hear(position);
            }
        }

        public void ReleaseProjectile(DistractionProjectile projectile)
        {
            _projectiles.Release(projectile.gameObject);
            _projectileInFlight = false;
        }

        public void ReleaseSmoke(SmokeCloud smoke)
        {
            _flashes.Release(smoke.gameObject);
        }

        Vector3 GroundPoint(Vector3 hit)
        {
            Vector3 origin = hit + Vector3.up * 0.3f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit ground, 8f, ~0, QueryTriggerInteraction.Ignore))
                return ground.point + Vector3.up * 0.02f;

            hit.y = 0.02f;
            return hit;
        }

        void Throw()
        {
            Vector3 hand = transform.TransformPoint(_handLocal);
            Vector3 direction = AimDirection(hand);
            if (!_projectiles.TrySpawn(hand, Quaternion.identity, out GameObject projectileObject))
                return;

            _projectileInFlight = true;
            DistractionProjectile projectile = projectileObject.GetComponent<DistractionProjectile>();
            projectile.Launch(this, direction * _speed, _projectileLife);
        }

        /// <summary>
        /// Starts at the hand and flies at the surface under the crosshair.
        /// </summary>
        Vector3 AimDirection(Vector3 hand)
        {
            Camera camera = _aim.GetComponent<Camera>();
            Ray aimRay = camera != null
                ? camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
                : new Ray(_aim.position, _aim.forward);

            Vector3 mark = CrosshairPoint(aimRay);
            Vector3 toMark = mark - hand;
            float sqr = toMark.sqrMagnitude;
            if (sqr < 1e-8f)
                return aimRay.direction;

            return toMark / Mathf.Sqrt(sqr);
        }

        Vector3 CrosshairPoint(Ray aimRay)
        {
            int count = Physics.RaycastNonAlloc(
                aimRay.origin,
                aimRay.direction,
                _aimHits,
                _aimDistance,
                ~0,
                QueryTriggerInteraction.Ignore);

            float closest = _aimDistance;
            Vector3 mark = aimRay.GetPoint(_aimDistance);
            for (int i = 0; i < count; i++)
            {
                Transform hit = _aimHits[i].transform;
                if (hit == transform || hit.IsChildOf(transform))
                    continue;
                if (_aimHits[i].distance >= closest)
                    continue;

                closest = _aimHits[i].distance;
                mark = _aimHits[i].point;
            }

            return mark;
        }

        void Resolve()
        {
            if (_throw != null || _actions == null)
                return;

            InputActionMap map = _actions.FindActionMap("Player", false);
            if (map == null)
                return;

            _throw = map.FindAction("Throw", false);
        }

        PrefabPool CreatePool<T>(
            Transform root,
            int capacity,
            float scale,
            Material material,
            bool projectile,
            out T[] scripts) where T : Component
        {
            var items = new GameObject[capacity];
            scripts = new T[capacity];
            for (int i = 0; i < capacity; i++)
            {
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.name = projectile ? "SmokeBomb" : "Smoke";
                sphere.transform.SetParent(root, false);
                sphere.transform.localScale = Vector3.one * scale;
                MeshRenderer renderer = sphere.GetComponent<MeshRenderer>();
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                if (material != null)
                    renderer.sharedMaterial = material;

                if (projectile)
                {
                    Rigidbody body = sphere.AddComponent<Rigidbody>();
                    body.useGravity = false;
                    body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    body.interpolation = RigidbodyInterpolation.Interpolate;
                    scripts[i] = sphere.AddComponent<DistractionProjectile>() as T;
                }
                else
                {
                    Destroy(sphere.GetComponent<Collider>());
                    sphere.GetComponent<MeshRenderer>().enabled = false;
                    SmokeCloud smoke = sphere.AddComponent<SmokeCloud>();
                    smoke.Bind(this, material, _smokeLife);
                    scripts[i] = smoke as T;
                }

                items[i] = sphere;
            }

            return new PrefabPool(items);
        }

        void IgnorePlayerColliders()
        {
            Collider[] playerColliders = GetComponents<Collider>();
            for (int i = 0; i < _projectileScripts.Length; i++)
            {
                Collider projectile = _projectileScripts[i].GetComponent<Collider>();
                for (int c = 0; c < playerColliders.Length; c++)
                    Physics.IgnoreCollision(projectile, playerColliders[c], true);
            }
        }
    }
}
