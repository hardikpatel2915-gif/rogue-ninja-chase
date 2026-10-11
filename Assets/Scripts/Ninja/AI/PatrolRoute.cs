using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    public sealed class PatrolRoute : MonoBehaviour
    {
        [SerializeField] Transform[] _points;

        Vector3[] _world;
        int _index;

        public bool HasPoints => _world != null && _world.Length > 0;
        public Vector3 Current => _world[_index];

        /// <summary>
        /// Copies points into world space so they do not move when the ninja does.
        /// </summary>
        public void Capture(Vector3 origin)
        {
            if (_points != null && _points.Length > 0)
            {
                _world = new Vector3[_points.Length];
                for (int i = 0; i < _points.Length; i++)
                    _world[i] = _points[i] != null ? _points[i].position : origin;
            }
            else
            {
                _world = new Vector3[4];
                _world[0] = origin;
                _world[1] = origin + new Vector3(0f, 0f, 4f);
                _world[2] = origin + new Vector3(3f, 0f, 4f);
                _world[3] = origin + new Vector3(3f, 0f, 0f);
            }

            _index = 0;
        }

        public void Advance()
        {
            if (!HasPoints)
                return;

            _index++;
            if (_index >= _world.Length)
                _index = 0;
        }
    }
}
