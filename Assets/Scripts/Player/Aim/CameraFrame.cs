namespace RogueNinjaChase.Player.Aim
{
    public readonly struct CameraFrame
    {
        public readonly CameraPose Pose;
        public readonly CameraFraming Framing;

        public CameraFrame(CameraPose pose, CameraFraming framing)
        {
            Pose = pose;
            Framing = framing;
        }
    }
}
