namespace RogueNinjaChase.Player.Aim
{
    /// <summary>
    /// Shoulder sits just off the right shoulder at eye height. Follow is centered on the body and tilted down so the full figure stays in frame.
    /// Distances match this model: hips near 0.33 m, head near 0.65 m.
    /// </summary>
    public readonly struct CameraFraming
    {
        public static readonly CameraFraming Shoulder = new CameraFraming(0.55f, 0.32f, 0.05f, 1.5f, 5f);
        public static readonly CameraFraming Follow = new CameraFraming(0.38f, 0f, 0.48f, 1.85f, 15f);

        public readonly float PivotHeight;
        public readonly float Side;
        public readonly float Height;
        public readonly float Distance;
        public readonly float LookDownDegrees;

        public CameraFraming(float pivotHeight, float side, float height, float distance, float lookDownDegrees)
        {
            PivotHeight = pivotHeight;
            Side = side;
            Height = height;
            Distance = distance;
            LookDownDegrees = lookDownDegrees;
        }

        public static CameraFraming For(CameraViewMode mode)
        {
            return mode == CameraViewMode.Follow ? Follow : Shoulder;
        }

        public static CameraViewMode Toggle(CameraViewMode mode)
        {
            return mode == CameraViewMode.Shoulder ? CameraViewMode.Follow : CameraViewMode.Shoulder;
        }

        public static bool AllowsAim(CameraViewMode mode)
        {
            return mode == CameraViewMode.Shoulder;
        }
    }
}
