using RogueNinjaChase.Player.Model;

namespace RogueNinjaChase.Player.Input
{
    /// <summary>
    /// Empty input used when no reader is on the player yet.
    /// </summary>
    public sealed class IdleMoveInputSource : IMoveInputSource
    {
        public static readonly IdleMoveInputSource Instance = new IdleMoveInputSource();

        IdleMoveInputSource()
        {
        }

        public MoveInput Read()
        {
            return default;
        }
    }
}
