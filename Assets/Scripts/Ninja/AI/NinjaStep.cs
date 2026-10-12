using UnityEngine;

namespace RogueNinjaChase.Ninja.AI
{
    public readonly struct NinjaStep
    {
        public readonly NinjaState State;
        public readonly NinjaClip Clip;
        public readonly bool AdvanceRoute;
        public readonly bool Catch;
        public readonly Color ConeColor;

        public NinjaStep(NinjaState state, NinjaClip clip, bool advanceRoute, bool catchPlayer, Color coneColor)
        {
            State = state;
            Clip = clip;
            AdvanceRoute = advanceRoute;
            Catch = catchPlayer;
            ConeColor = coneColor;
        }
    }
}
