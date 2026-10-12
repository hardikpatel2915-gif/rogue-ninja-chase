using UnityEngine;
using UnityEngine.SceneManagement;

namespace RogueNinjaChase.Mission
{
    public sealed class MissionFlow : MonoBehaviour
    {
        [SerializeField] float _restartDelay = 2.5f;
        [SerializeField] MissionHud _hudPrefab;
        [SerializeField] ControlHints _hintsPrefab;

        readonly MissionModel _model = new MissionModel();
        MissionHud _hud;
        float _restartTimer;
        bool _reloading;

        public static MissionFlow Instance { get; private set; }
        public MissionPhase Phase => _model.Phase;

        void Awake()
        {
            Instance = this;
            if (_hudPrefab != null)
                _hud = Instantiate(_hudPrefab);
            if (_hintsPrefab != null)
                Instantiate(_hintsPrefab);
        }

        void Start()
        {
            if (_hud != null)
                _hud.Show(_model.Phase);
        }

        void Update()
        {
            if (!_model.IsComplete || _reloading)
                return;

            // timeScale is 0 after the ending, so the delay uses unscaled time.
            _restartTimer += Time.unscaledDeltaTime;
            if (_restartTimer < _restartDelay)
                return;

            _reloading = true;
            // A frozen scale would carry into the reloaded scene.
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public bool TryPickup()
        {
            return Change(_model.TryPickup());
        }

        public bool TryExtract()
        {
            return Change(_model.TryExtract());
        }

        public bool TryCatch()
        {
            return Change(_model.TryCatch());
        }

        bool Change(bool changed)
        {
            if (!changed)
                return false;

            if (_hud != null)
                _hud.Show(_model.Phase);
            if (_model.IsComplete)
                Time.timeScale = 0f;
            return true;
        }

        void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}
