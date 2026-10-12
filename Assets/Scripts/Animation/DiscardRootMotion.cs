using UnityEngine;

namespace RogueNinjaChase.Animation
{
    /// <summary>
    /// Discards root motion so the mesh can be held on the body.
    /// </summary>
    public sealed class DiscardRootMotion : MonoBehaviour
    {
        Animator _animator;
        Transform _hips;
        Vector3 _bindPosition;
        Quaternion _bindRotation;
        Vector3 _hipBindLocal;
        bool _hipCaptured;

        public static void Attach(Animator animator)
        {
            if (animator == null || animator.GetComponent<DiscardRootMotion>() != null)
                return;

            animator.gameObject.AddComponent<DiscardRootMotion>();
        }

        void Awake()
        {
            _bindPosition = transform.localPosition;
            _bindRotation = transform.localRotation;
            _animator = GetComponent<Animator>();
            if (_animator != null)
                _animator.applyRootMotion = true;
            CaptureHips();
        }

        void Start()
        {
            CaptureHips();
        }

        void CaptureHips()
        {
            if (_hipCaptured || _animator == null)
                return;

            _hips = _animator.GetBoneTransform(HumanBodyBones.Hips);
            if (_hips == null)
                return;

            _hipBindLocal = transform.InverseTransformPoint(_hips.position);
            _hipCaptured = true;
        }

        void OnAnimatorMove()
        {
        }

        void LateUpdate()
        {
            transform.localPosition = _bindPosition;
            transform.localRotation = _bindRotation;
            if (_hips == null)
                return;

            Vector3 hipsLocal = transform.InverseTransformPoint(_hips.position);
            Vector3 slide = hipsLocal - _hipBindLocal;
            slide.y = 0f;
            transform.localPosition = _bindPosition - slide;
        }
    }
}
