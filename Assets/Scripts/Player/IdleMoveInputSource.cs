namespace RogueNinjaChase.Player
{
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
