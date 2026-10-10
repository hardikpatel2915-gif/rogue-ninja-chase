using UnityEngine;

namespace RogueNinjaChase.Core
{
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
