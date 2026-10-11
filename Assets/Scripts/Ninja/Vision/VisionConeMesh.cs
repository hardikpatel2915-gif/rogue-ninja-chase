using UnityEngine;
using UnityEngine.Rendering;

namespace RogueNinjaChase.Ninja.Vision
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    /// <summary>
    /// Floor wedge. Spoke rays stay in world space; the mesh stores local points
    /// because this transform is applied again when the mesh is drawn.
    /// </summary>
    public sealed class VisionConeMesh : MonoBehaviour
    {
        const int Segments = 24;
        const float FloorLift = 0.05f;

        [SerializeField] Material _material;

        static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        Mesh _mesh;
        MeshFilter _filter;
        MeshRenderer _renderer;
        Vector3[] _vertices;
        readonly RaycastHit[] _spokeHits = new RaycastHit[8];
        MaterialPropertyBlock _colorBlock;
        Color _tint;
        bool _hasTint;

        void Awake()
        {
            _filter = GetComponent<MeshFilter>();
            if (_filter == null)
                _filter = gameObject.AddComponent<MeshFilter>();
            _renderer = GetComponent<MeshRenderer>();
            if (_renderer == null)
                _renderer = gameObject.AddComponent<MeshRenderer>();
            _renderer.shadowCastingMode = ShadowCastingMode.Off;
            _renderer.receiveShadows = false;
            if (_material != null)
                _renderer.sharedMaterial = _material;

            _vertices = new Vector3[Segments + 1];
            _mesh = new Mesh();
            _mesh.name = "VisionCone";
            _mesh.MarkDynamic();
            _mesh.SetVertices(_vertices);
            _mesh.SetTriangles(BuildTriangles(), 0);
            _filter.sharedMesh = _mesh;
            _colorBlock = new MaterialPropertyBlock();
        }

        public void SetColor(Color color)
        {
            if (_renderer == null || (_hasTint && _tint == color))
                return;

            _hasTint = true;
            _tint = color;
            _renderer.GetPropertyBlock(_colorBlock);
            _colorBlock.SetColor(BaseColorId, color);
            _renderer.SetPropertyBlock(_colorBlock);
        }

        public void Show(
            Vector3 origin,
            Vector3 forward,
            float distance,
            float viewAngleDegrees,
            LayerMask occlusion)
        {
            if (_mesh == null || forward.sqrMagnitude < 1e-8f)
                return;

            Vector3 center = origin;
            center.y += FloorLift;
            // World points written as local vertices, or the ninja transform shifts the cone a second time.
            _vertices[0] = transform.InverseTransformPoint(center);

            float half = viewAngleDegrees * 0.5f * Mathf.Deg2Rad;
            float start = -half;
            float step = viewAngleDegrees * Mathf.Deg2Rad / Segments;
            Vector3 right = new Vector3(forward.z, 0f, -forward.x);

            for (int i = 0; i < Segments; i++)
            {
                float angle = start + step * i;
                float sin = Mathf.Sin(angle);
                float cos = Mathf.Cos(angle);
                Vector3 direction = forward * cos + right * sin;
                float length = SpokeLength(center, direction, distance, occlusion);
                Vector3 world = center + direction * length;
                _vertices[i + 1] = transform.InverseTransformPoint(world);
            }

            _mesh.SetVertices(_vertices);
            _mesh.RecalculateBounds();
        }

        float SpokeLength(Vector3 origin, Vector3 direction, float distance, LayerMask occlusion)
        {
            int count = Physics.RaycastNonAlloc(
                origin,
                direction,
                _spokeHits,
                distance,
                occlusion,
                QueryTriggerInteraction.Ignore);

            float closest = distance;
            for (int i = 0; i < count; i++)
            {
                Transform hit = _spokeHits[i].transform;
                if (hit == transform || hit.IsChildOf(transform))
                    continue;
                if (_spokeHits[i].distance < closest)
                    closest = _spokeHits[i].distance;
            }

            return closest;
        }

        static int[] BuildTriangles()
        {
            int[] triangles = new int[Segments * 3];
            for (int i = 0; i < Segments; i++)
            {
                int next = i == Segments - 1 ? 1 : i + 2;
                int write = i * 3;
                triangles[write] = 0;
                triangles[write + 1] = i + 1;
                triangles[write + 2] = next;
            }

            return triangles;
        }
    }
}
