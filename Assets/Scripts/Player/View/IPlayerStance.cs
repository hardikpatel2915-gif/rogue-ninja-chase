namespace RogueNinjaChase.Player.View
{
    /// <summary>
    /// Lets vision read a crouch without depending on the motor.
    /// </summary>
    public interface IPlayerStance
    {
        bool IsCrouching { get; }
    }
}
