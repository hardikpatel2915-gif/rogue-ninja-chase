namespace RogueNinjaChase.Player.Input
{
    public readonly struct LookDelta
    {
        public readonly float X;
        public readonly float Y;

        public LookDelta(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
