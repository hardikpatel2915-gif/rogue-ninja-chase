namespace RogueNinjaChase.Ninja.AI
{
    /// <summary>
    /// Picks a heading around an obstacle and keeps that side until the straight line is clear.
    /// Sample 0 is straight at the goal. The next samples are increasing angles to one side, then those angles to the other.
    /// </summary>
    public static class DetourSelect
    {
        public const int AngleCount = 4;
        public const int SampleCount = 1 + AngleCount * 2;

        public static readonly float[] Angles = { 40f, 75f, 110f, 150f };

        public static int Select(float[] clearance, int committedSide, float needed)
        {
            if (clearance[0] >= needed)
                return 0;

            if (committedSide != 0 && TrySide(clearance, committedSide, needed, out int committed))
                return committed;

            int positive = -1;
            int negative = -1;
            TrySide(clearance, 1, needed, out positive);
            TrySide(clearance, -1, needed, out negative);
            if (positive >= 0 && negative >= 0)
                return positive - 1 <= negative - AngleCount - 1 ? positive : negative;
            if (positive >= 0)
                return positive;
            if (negative >= 0)
                return negative;

            int best = 0;
            float bestScore = clearance[0];
            for (int i = 1; i < clearance.Length; i++)
            {
                float score = clearance[i];
                if (SideOf(i) == committedSide && committedSide != 0)
                    score += 0.4f;
                if (score > bestScore)
                {
                    best = i;
                    bestScore = score;
                }
            }

            return best;
        }

        public static int SideOf(int index)
        {
            if (index <= 0)
                return 0;
            return index <= AngleCount ? 1 : -1;
        }

        public static float Degrees(int index)
        {
            if (index <= 0)
                return 0f;
            if (index <= AngleCount)
                return Angles[index - 1];
            return -Angles[index - AngleCount - 1];
        }

        static bool TrySide(float[] clearance, int side, float needed, out int index)
        {
            int start = side > 0 ? 1 : AngleCount + 1;
            int end = start + AngleCount;
            for (int i = start; i < end; i++)
            {
                if (clearance[i] < needed)
                    continue;

                index = i;
                return true;
            }

            index = -1;
            return false;
        }
    }
}
