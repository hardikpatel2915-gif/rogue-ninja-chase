using UnityEngine;

namespace RogueNinjaChase.Core
{
    /// <summary>
    /// Caps the player at 60. vSync has to stay off or this cap is ignored.
    /// </summary>
    static class FrameRate
    {
        const int Target = 60;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Apply()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = Target;
        }
    }
}
