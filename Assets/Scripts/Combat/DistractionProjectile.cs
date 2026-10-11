using UnityEngine;

namespace RogueNinjaChase.Combat
{
    public sealed class DistractionProjectile : MonoBehaviour
    {
        DistractionLoadout _owner;
        float _life;
        bool _live;

        public void Launch(DistractionLoadout owner, Vector3 velocity, float life)
        {
            _owner = owner;
            _life = life;
            _live = true;
            GetComponent<Rigidbody>().linearVelocity = velocity;
        }

        void Update()
        {
            if (!_live)
                return;

            _life -= Time.deltaTime;
            if (_life <= 0f)
                Finish(transform.position, false);
        }

        void OnCollisionEnter(Collision collision)
        {
            if (!_live)
                return;

            Finish(transform.position, true);
        }

        void Finish(Vector3 position, bool impact)
        {
            _live = false;
            Rigidbody body = GetComponent<Rigidbody>();
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
            if (impact)
                _owner.Impact(position);
            _owner.ReleaseProjectile(this);
        }
    }
}
