namespace RogueNinjaChase.Player.Model
{
    /// <summary>
    /// Crouch is half as tall and a narrower radius. The center drops so the feet stay put.
    /// </summary>
    public readonly struct CapsulePose
    {
        public const float CrouchHeightScale = 0.5f;
        public const float CrouchRadiusScale = 0.45f;

        public readonly float Height;
        public readonly float Radius;
        public readonly float CenterY;
        public readonly bool IsCrouching;

        public CapsulePose(float height, float radius, float centerY, bool isCrouching)
        {
            Height = height;
            Radius = radius;
            CenterY = centerY;
            IsCrouching = isCrouching;
        }

        public static CapsulePose From(
            bool crouching,
            float standingHeight,
            float standingRadius,
            float standingCenterY)
        {
            float height = crouching ? standingHeight * CrouchHeightScale : standingHeight;
            float radius = crouching ? standingRadius * CrouchRadiusScale : standingRadius;
            float centerY = crouching
                ? standingCenterY - (standingHeight - height) * 0.5f
                : standingCenterY;
            return new CapsulePose(height, radius, centerY, crouching);
        }
    }
}
