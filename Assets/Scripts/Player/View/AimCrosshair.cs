using UnityEngine;
using UnityEngine.UI;

namespace RogueNinjaChase.Player.View
{
    /// <summary>
    /// Four ticks around the center of the screen. Built once, then left alone.
    /// </summary>
    public sealed class AimCrosshair : MonoBehaviour
    {
        const float Gap = 6f;
        const float Length = 10f;
        const float Thickness = 2f;

        static Sprite _pixel;

        void Awake()
        {
            Sprite pixel = Pixel();
            var canvasObject = new GameObject("AimCrosshair");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            RectTransform root = canvasObject.GetComponent<RectTransform>();
            CreateTick(root, pixel, new Vector2(-Gap, 0f), new Vector2(Length, Thickness), new Vector2(1f, 0.5f));
            CreateTick(root, pixel, new Vector2(Gap, 0f), new Vector2(Length, Thickness), new Vector2(0f, 0.5f));
            CreateTick(root, pixel, new Vector2(0f, Gap), new Vector2(Thickness, Length), new Vector2(0.5f, 0f));
            CreateTick(root, pixel, new Vector2(0f, -Gap), new Vector2(Thickness, Length), new Vector2(0.5f, 1f));
        }

        static void CreateTick(RectTransform parent, Sprite pixel, Vector2 position, Vector2 size, Vector2 pivot)
        {
            var tick = new GameObject("Tick");
            tick.transform.SetParent(parent, false);
            var image = tick.AddComponent<Image>();
            image.sprite = pixel;
            image.color = new Color(1f, 1f, 1f, 0.9f);
            image.raycastTarget = false;

            RectTransform rect = image.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        static Sprite Pixel()
        {
            if (_pixel != null)
                return _pixel;

            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            _pixel = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
            return _pixel;
        }
    }
}
