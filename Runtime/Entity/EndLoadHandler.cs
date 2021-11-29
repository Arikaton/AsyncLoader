namespace LoadingModule.Entity
{
    public readonly struct EndLoadHandler
    {
        public readonly bool IsSuccessful;

        private EndLoadHandler(bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
        }

        public static implicit operator bool(EndLoadHandler h) => h.IsSuccessful;
        public static implicit operator EndLoadHandler(bool b) => new EndLoadHandler(b);
    }
}
