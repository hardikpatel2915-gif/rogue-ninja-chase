using UnityEngine;

namespace RogueNinjaChase.Player.View
{
    /// <summary>
    /// The assigned sprite sits at the center of the screen. Throw aims through that point.
    /// </summary>
    public sealed class AimCrosshair : MonoBehaviour
    {
        [SerializeField] RectTransform _mark;

        Transform _canvasRoot;

        void Awake()
        {
            if (_mark == null)
                return;

            // A child of the player would move and scale with the body. The mark has to stay on the screen.
            _canvasRoot = _mark.parent;
            if (_canvasRoot == null)
                return;

            _canvasRoot.SetParent(null, false);
            if (_canvasRoot is not RectTransform canvas)
                return;

            canvas.localScale = Vector3.one;
            canvas.anchorMin = Vector2.zero;
            canvas.anchorMax = Vector2.one;
            canvas.offsetMin = Vector2.zero;
            canvas.offsetMax = Vector2.zero;
            canvas.anchoredPosition = Vector2.zero;
        }

        void OnDestroy()
        {
            if (_canvasRoot != null)
                Destroy(_canvasRoot.gameObject);
        }

        public Vector2 AimScreenPoint
        {
            get
            {
                if (_mark == null)
                    return new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

                Vector3 point = _mark.position;
                return new Vector2(point.x, point.y);
            }
        }

        public void SetVisible(bool visible)
        {
            if (_mark != null && _mark.gameObject.activeSelf != visible)
                _mark.gameObject.SetActive(visible);
        }
    }
}
