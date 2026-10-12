namespace RogueNinjaChase.Ninja.AI
{
    public enum NinjaClip
    {
        Patrol = 0,
        Investigate = 1,
        InvestigateIdle = 2,
        Chase = 3
    }

    /// <summary>
    /// Patrol, investigate, and chase each have their own clip. The investigate wait does not keep walking.
    /// </summary>
    public static class NinjaClipSelect
    {
        public static NinjaClip Select(NinjaState state, bool moving)
        {
            switch (state)
            {
                case NinjaState.Chase:
                    return NinjaClip.Chase;
                case NinjaState.Investigate:
                    return moving ? NinjaClip.Investigate : NinjaClip.InvestigateIdle;
                default:
                    return NinjaClip.Patrol;
            }
        }
    }
}
