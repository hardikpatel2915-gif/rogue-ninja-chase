namespace RogueNinjaChase.Mission
{
    /// <summary>
    /// Pickup, then the dock. A catch fails the run unless the dossier is already out.
    /// </summary>
    public sealed class MissionModel
    {
        public MissionPhase Phase { get; private set; } = MissionPhase.Infiltrate;

        public bool IsComplete => Phase == MissionPhase.Extracted || Phase == MissionPhase.Caught;

        public bool TryPickup()
        {
            if (Phase != MissionPhase.Infiltrate)
                return false;

            Phase = MissionPhase.Carrying;
            return true;
        }

        public bool TryExtract()
        {
            if (Phase != MissionPhase.Carrying)
                return false;

            Phase = MissionPhase.Extracted;
            return true;
        }

        public bool TryCatch()
        {
            if (IsComplete)
                return false;

            Phase = MissionPhase.Caught;
            return true;
        }
    }
}
