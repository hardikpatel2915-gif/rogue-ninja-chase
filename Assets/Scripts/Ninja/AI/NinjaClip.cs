namespace RogueNinjaChase.Ninja.AI
{
    public enum NinjaClip
    {
        Patrol = 0,
        Investigate = 1,
        Chase = 2
    }

    /// <summary>
    /// The run to an investigation point uses the chase clip. Investigate plays only once that point is reached.
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
                    return moving ? NinjaClip.Chase : NinjaClip.Investigate;
                default:
                    return NinjaClip.Patrol;
            }
        }
    }
}
