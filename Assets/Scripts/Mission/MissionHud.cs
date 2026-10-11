using UnityEngine;
using UnityEngine.UI;

namespace RogueNinjaChase.Mission
{
    /// <summary>
    /// Writes the objective only when the phase changes.
    /// </summary>
    public sealed class MissionHud : MonoBehaviour
    {
        const string Seek = "Recover the dossier";
        const string Carry = "Reach the dock handover";
        const string Won = "Dossier secured";
        const string Lost = "Caught. The documents stay behind.";

        Text _objective;
        MissionPhase _shown;

        void Awake()
        {
            _objective = CreateText();
            _shown = (MissionPhase)(-1);
        }

        public void Show(MissionPhase phase)
        {
            if (_objective == null || phase == _shown)
                return;

            _shown = phase;
            if (phase == MissionPhase.Carrying)
                _objective.text = Carry;
            else if (phase == MissionPhase.Extracted)
                _objective.text = Won;
            else if (phase == MissionPhase.Caught)
                _objective.text = Lost;
            else
                _objective.text = Seek;
        }

        static Text CreateText()
        {
            var canvasObject = new GameObject("MissionHud");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();

            var textObject = new GameObject("Objective");
            textObject.transform.SetParent(canvasObject.transform, false);
            var text = textObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 28;
            text.alignment = TextAnchor.UpperCenter;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;

            var rect = text.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -28f);
            rect.sizeDelta = new Vector2(900f, 48f);
            return text;
        }
    }
}
