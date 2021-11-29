namespace LoadingModule.Entity
{
    public sealed class LoadingError
    {
        public string Message { get; }

        internal LoadingError(string message, string stacktrace)
        {
            Message = $"{Constants.LoadingModuleTag} {message}\n{stacktrace}";
        }
    }
}