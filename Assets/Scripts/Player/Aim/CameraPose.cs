namespace RogueNinjaChase.Player.Aim
{
    public readonly struct CameraPose
    {
        public readonly float YawDegrees;
        public readonly float PitchDegrees;

        public CameraPose(float yawDegrees, float pitchDegrees)
        {
            YawDegrees = yawDegrees;
            PitchDegrees = pitchDegrees;
        }
    }
}