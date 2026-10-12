namespace RogueNinjaChase.Mission
{
    public interface IMission
    {
        MissionPhase Phase { get; }
        bool TryPickup();
        bool TryExtract();
        bool TryCatch();
    }
}
