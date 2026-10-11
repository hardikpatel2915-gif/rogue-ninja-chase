namespace RogueNinjaChase.Player.Aim
{
    /// <summary>
    /// Shoulder sits to the right and looks ahead. Follow is centered, higher, and tilted down.
    /// </summary>
    public readonly struct CameraFraming
    {
        public static readonly CameraFraming Shoulder = new CameraFraming(1.6f, 0.45f, 0f, 3.5f, 0f);
        public static readonly CameraFraming Follow = new CameraFraming(1.15f, 0f, 1.35f, 5.2f, 16f);

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
    }
}
