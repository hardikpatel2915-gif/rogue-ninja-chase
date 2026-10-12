namespace RogueNinjaChase.Player.Model
{
    public readonly struct JumpCommand
    {
        public readonly bool SetVerticalSpeed;
        public readonly float VerticalSpeed;
        public readonly bool IsJumping;

        public JumpCommand(bool setVerticalSpeed, float verticalSpeed, bool isJumping)
        {
            SetVerticalSpeed = setVerticalSpeed;
            VerticalSpeed = verticalSpeed;
            IsJumping = isJumping;
        }
    }
}
