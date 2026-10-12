using UnityEngine;
using UnityEngine.UI;

namespace RogueNinjaChase.Mission
{
    /// <summary>
    /// Writes the objective only when the phase changes. The text object lives on the prefab.
    /// </summary>
    public sealed class MissionHud : MonoBehaviour
    {
        const string Seek = "Find the chest and collect the dossier";
        const string Carry = "Exit the building and hand the dossier over at the dock";
        const string Won = "Dossier secured";
        const string Lost = "Mission failed";
        const string CaughtTitle = "CAUGHT";
        const string SecuredTitle = "SECURED";
        const float BannerDuration = 0.45f;
        const float FpsWindow = 0.5f;
        const int FpsLimit = 240;

        static readonly string[] FpsLabels = BuildFpsLabels();

        static readonly Color CaughtTitleColor = new Color(0.95f, 0.22f, 0.18f, 1f);
        static readonly Color SecuredTitleColor = new Color(1f, 0.84f, 0.28f, 1f);

        [SerializeField] Text _objective;
        [SerializeField] Text _fps;
        [SerializeField] GameObject _caught;
        [SerializeField] Image _caughtPanel;
        [SerializeField] Text _caughtTitle;
        MissionPhase _shown;
        float _caughtTime;
        bool _playingCaught;
        Color _panelColor;
        Color _titleColor;
        float _fpsTime;
        int _fpsFrames;
        int _shownFps = -1;

        void Awake()
        {
            _shown = (MissionPhase)(-1);
            if (_caught != null)
                _caught.SetActive(false);
            if (_caughtPanel != null)
                _panelColor = _caughtPanel.color;
            if (_caughtTitle != null)
                _titleColor = _caughtTitle.color;
        }

        void Update()
        {
            TickFps();
            if (!_playingCaught)
                return;

            _caughtTime += Time.unscaledDeltaTime;
            float t = _caughtTime / BannerDuration;
            if (t > 1f)
                t = 1f;
            float eased = 1f - (1f - t) * (1f - t);
            if (_caughtPanel != null)
            {
                Color panel = _panelColor;
                panel.a = _panelColor.a * eased;
                _caughtPanel.color = panel;
            }

            if (_caughtTitle != null)
            {
                Color title = _titleColor;
                title.a = _titleColor.a * eased;
                _caughtTitle.color = title;
                _caughtTitle.rectTransform.localScale = Vector3.one * (1.4f - 0.4f * eased);
            }

            if (t >= 1f)
                _playingCaught = false;
        }

        void TickFps()
        {
            if (_fps == null)
                return;

            _fpsFrames++;
            _fpsTime += Time.unscaledDeltaTime;
            if (_fpsTime < FpsWindow)
                return;

            int fps = (int)(_fpsFrames / _fpsTime + 0.5f);
            _fpsFrames = 0;
            _fpsTime = 0f;
            if (fps < 0)
                fps = 0;
            if (fps > FpsLimit)
                fps = FpsLimit;
            if (fps == _shownFps)
                return;

            _shownFps = fps;
            _fps.text = FpsLabels[fps];
        }

        static string[] BuildFpsLabels()
        {
            var labels = new string[FpsLimit + 1];
            for (int i = 0; i <= FpsLimit; i++)
                labels[i] = i + " FPS";
            return labels;
        }

        public void Show(MissionPhase phase)
        {
            if (_objective == null || phase == _shown)
                return;

            _shown = phase;
            if (phase == MissionPhase.Carrying)
                _objective.text = Carry;
            else if (phase == MissionPhase.Extracted)
            {
                _objective.text = Won;
                PlayBanner(SecuredTitle, SecuredTitleColor);
            }
            else if (phase == MissionPhase.Caught)
            {
                _objective.text = Lost;
                PlayBanner(CaughtTitle, CaughtTitleColor);
            }
            else
                _objective.text = Seek;
        }

        void PlayBanner(string title, Color titleColor)
        {
            if (_caught == null)
                return;

            if (_caughtTitle != null)
            {
                _caughtTitle.text = title;
                _titleColor = titleColor;
            }

            _caught.SetActive(true);
            _caughtTime = 0f;
            _playingCaught = true;
            if (_caughtPanel != null)
            {
                Color panel = _panelColor;
                panel.a = 0f;
                _caughtPanel.color = panel;
            }

            if (_caughtTitle != null)
            {
                Color color = _titleColor;
                color.a = 0f;
                _caughtTitle.color = color;
                _caughtTitle.rectTransform.localScale = Vector3.one * 1.4f;
            }
        }
    }
}
