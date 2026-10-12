namespace RogueNinjaChase.Player.Aim
{
    /// <summary>
    /// Escape unlocks the pointer. A click locks it again. Starts locked.
    /// </summary>
    public sealed class CursorLockModel
    {
        bool _locked = true;

        public bool Locked => _locked;

        public bool Tick(bool escapePressed, bool clickPressed)
        {
            if (escapePressed)
            {
                _locked = false;
                return _locked;
            }

            if (!_locked && clickPressed)
                _locked = true;

            return _locked;
        }

        public void Lock()
        {
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
        }
    }
}
