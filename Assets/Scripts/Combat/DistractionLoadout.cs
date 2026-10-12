using RogueNinjaChase.Core;
using RogueNinjaChase.Player.Input;
using RogueNinjaChase.Player.Presenter;
using RogueNinjaChase.Player.View;
using RogueNinjaChase.Pool;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

namespace RogueNinjaChase.Combat
{
    /// <summary>
    /// Q, F, or the right mouse button throws the smoke bomb in the close camera.
    /// </summary>
    [DefaultExecutionOrder(100)]
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
        [SerializeField] string _mapName = "Player";
        [SerializeField] string _throwName = "Throw";
        [SerializeField] string _throwState = "Throw";
        [SerializeField] string _handBone = "bip001 r hand";

        const float ReleaseNormalized = 0.32f;

        PrefabPool _projectiles;
        PrefabPool _flashes;
        DistractionProjectile[] _projectileScripts;
        readonly RaycastHit[] _aimHits = new RaycastHit[8];
        INoiseListener[] _listeners;
        InputAction _throw;
        AimCrosshair _crosshair;
        Transform _aim;
        PlayerLocomotion _locomotion;
        ShoulderCamera _camera;
        Animator _animator;
        Transform _hand;
        int _throwHash;
        float _noiseSqr;
        bool _projectileInFlight;
        bool _throwPending;
        bool _throwQueued;
        bool _keyHeld;
        bool _rightHeld;
        float _throwWait;

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
            _projectiles = CreatePool(root, _capacity, 0.04f, _projectileMaterial, true, out _projectileScripts);
            _flashes = CreatePool(root, _capacity, 0.45f, _smokeMaterial, false, out SmokeCloud[] _);
            IgnorePlayerColliders();
            _crosshair = GetComponent<AimCrosshair>();
            _locomotion = GetComponent<PlayerLocomotion>();
            _camera = GetComponent<ShoulderCamera>();
            _throwHash = Animator.StringToHash(_throwState);
            Resolve();
        }

        void Start()
        {
            _listeners = FindListeners();
            Camera childCamera = GetComponentInChildren<Camera>();
            _aim = childCamera != null ? childCamera.transform : transform;
            _hand = FindThrowHand();
        }

        void OnEnable()
        {
            Resolve();
            if (_throw == null)
                return;

            _throw.performed += OnThrow;
            _throw.Enable();
        }

        void OnDisable()
        {
            if (_throw == null)
                return;

            _throw.performed -= OnThrow;
        }

        void OnThrow(InputAction.CallbackContext _)
        {
            _throwQueued = true;
        }

        void Update()
        {
            if (PressedThrow())
                _throwQueued = true;

            if (!CanThrow())
            {
                _throwPending = false;
                return;
            }

            if (_projectileInFlight || _throwPending || !_throwQueued)
                return;

            _throwQueued = false;
            if (_locomotion != null)
                _locomotion.NotifyThrow();
            _throwPending = true;
            _throwWait = 0f;
        }

        bool CanThrow()
        {
            return _camera == null || _camera.Aiming;
        }

        bool PressedThrow()
        {
            Keyboard keyboard = Keyboard.current;
            bool keyDown = keyboard != null && (keyboard.qKey.isPressed || keyboard.fKey.isPressed);
            bool keyPressed = keyDown && !_keyHeld;
            _keyHeld = keyDown;

            Mouse mouse = Mouse.current;
            bool rightDown = mouse != null && mouse.rightButton.isPressed;
            bool rightPressed = rightDown && !_rightHeld;
            _rightHeld = rightDown;

            return keyPressed || rightPressed;
        }

        void LateUpdate()
        {
            if (!CanThrow())
            {
                _throwPending = false;
                return;
            }

            if (!_throwPending)
                return;

            _throwWait += Time.deltaTime;
            // Release is a point in the clip, so a faster Throw state lets go sooner.
            if (!ReachedRelease() && _throwWait < 1.5f)
                return;

            _throwPending = false;
            Throw();
        }

        bool ReachedRelease()
        {
            if (_animator == null)
                return true;

            AnimatorStateInfo info = _animator.IsInTransition(0)
                ? _animator.GetNextAnimatorStateInfo(0)
                : _animator.GetCurrentAnimatorStateInfo(0);
            return info.shortNameHash == _throwHash && info.normalizedTime >= ReleaseNormalized;
        }

        public void Impact(Vector3 position)
        {
            Vector3 ground = GroundPoint(position);
            if (_flashes.TrySpawn(ground, Quaternion.identity, out GameObject smokeObject))
            {
                SmokeCloud smoke = smokeObject.GetComponent<SmokeCloud>();
                smoke.Play(ground);
            }

            if (_listeners == null)
                return;

            for (int i = 0; i < _listeners.Length; i++)
            {
                Vector3 delta = position - _listeners[i].Position;
                if (delta.sqrMagnitude <= _noiseSqr)
                    _listeners[i].Hear(position);
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
            Vector3 hand = _hand != null ? _hand.position : transform.TransformPoint(_handLocal);
            Vector3 direction = AimDirection(hand);
            if (!_projectiles.TrySpawn(hand, Quaternion.identity, out GameObject projectileObject))
                return;

            _projectileInFlight = true;
            DistractionProjectile projectile = projectileObject.GetComponent<DistractionProjectile>();
            projectile.Launch(this, direction * _speed, _projectileLife);
        }

        Transform FindThrowHand()
        {
            _animator = GetComponentInChildren<Animator>(true);
            if (_animator != null && _animator.isHuman)
                return _animator.GetBoneTransform(HumanBodyBones.RightHand);

            return FindNamed(transform, _handBone);
        }

        static Transform FindNamed(Transform root, string boneName)
        {
            if (root.name == boneName)
                return root;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform found = FindNamed(root.GetChild(i), boneName);
                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// Starts at the hand and flies at the surface under the aim sprite.
        /// </summary>
        Vector3 AimDirection(Vector3 hand)
        {
            Camera camera = _aim.GetComponent<Camera>();
            Ray aimRay = camera != null
                ? camera.ScreenPointToRay(AimScreenPoint(camera))
                : new Ray(_aim.position, _aim.forward);

            Vector3 mark = CrosshairPoint(aimRay);
            Vector3 toMark = mark - hand;
            float sqr = toMark.sqrMagnitude;
            if (sqr < 1e-8f)
                return aimRay.direction;

            return toMark / Mathf.Sqrt(sqr);
        }

        Vector3 AimScreenPoint(Camera camera)
        {
            if (_crosshair == null)
                return new Vector2(camera.pixelWidth * 0.5f, camera.pixelHeight * 0.5f);

            // The overlay places the sprite during the canvas update. Read it after that, or the ray uses the player's position.
            Canvas.ForceUpdateCanvases();
            return _crosshair.AimScreenPoint;
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

        static INoiseListener[] FindListeners()
        {
            MonoBehaviour[] behaviours = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
            int count = 0;
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is INoiseListener)
                    count++;
            }

            var listeners = new INoiseListener[count];
            int write = 0;
            for (int i = 0; i < behaviours.Length; i++)
            {
                if (behaviours[i] is INoiseListener listener)
                    listeners[write++] = listener;
            }

            return listeners;
        }

        void Resolve()
        {
            if (_throw != null || _actions == null)
                return;

            _throw = ActionBinding.Find(_actions, _mapName, _throwName);
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
                GameObject sphere = MakeSphere(projectile ? "SmokeBomb" : "Smoke");
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

        static Mesh _sphereMesh;

        static GameObject MakeSphere(string objectName)
        {
            var sphere = new GameObject(objectName);
            var filter = sphere.AddComponent<MeshFilter>();
            filter.sharedMesh = SharedSphere();
            sphere.AddComponent<MeshRenderer>();
            sphere.AddComponent<SphereCollider>();
            return sphere;
        }

        static Mesh SharedSphere()
        {
            if (_sphereMesh != null)
                return _sphereMesh;

            const int slices = 12;
            const int stacks = 8;
            int vertCount = (stacks + 1) * (slices + 1);
            var vertices = new Vector3[vertCount];
            var triangles = new int[stacks * slices * 6];
            int vertex = 0;
            for (int stack = 0; stack <= stacks; stack++)
            {
                float v = stack / (float)stacks;
                float phi = Mathf.PI * v;
                for (int slice = 0; slice <= slices; slice++)
                {
                    float u = slice / (float)slices;
                    float theta = Mathf.PI * 2f * u;
                    vertices[vertex++] = new Vector3(
                        0.5f * Mathf.Sin(phi) * Mathf.Cos(theta),
                        0.5f * Mathf.Cos(phi),
                        0.5f * Mathf.Sin(phi) * Mathf.Sin(theta));
                }
            }

            int triangle = 0;
            int row = slices + 1;
            for (int stack = 0; stack < stacks; stack++)
            {
                for (int slice = 0; slice < slices; slice++)
                {
                    int start = stack * row + slice;
                    triangles[triangle++] = start;
                    triangles[triangle++] = start + row;
                    triangles[triangle++] = start + 1;
                    triangles[triangle++] = start + 1;
                    triangles[triangle++] = start + row;
                    triangles[triangle++] = start + row + 1;
                }
            }

            _sphereMesh = new Mesh();
            _sphereMesh.name = "SmokeSphere";
            _sphereMesh.vertices = vertices;
            _sphereMesh.triangles = triangles;
            _sphereMesh.RecalculateNormals();
            return _sphereMesh;
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
