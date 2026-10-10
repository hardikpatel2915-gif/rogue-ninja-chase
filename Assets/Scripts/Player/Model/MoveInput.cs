namespace RogueNinjaChase.Player.Model
{
    public readonly struct MoveInput
    {
        public readonly float Strafe;
        public readonly float Forward;
        public readonly bool Sprint;
        public readonly bool Crouch;

        public MoveInput(float strafe, float forward, bool sprint, bool crouch)
        {
            Strafe = strafe;
            Forward = forward;
            Sprint = sprint;
            Crouch = crouch;
        }
    }
}
