using System;

namespace LoadingModule.Entity
{
    public sealed class EventSystem
    {
        /// <summary>
        /// Called every time when step loaded.
        /// </summary>
        public event Action<float> OnLoadingProgress;
        /// <summary>
        /// Called when error occured.
        /// </summary>
        public event Action<LoadingError> OnError;
        /// <summary>
        /// Called when loading finished. Return true if loading were successful (mean no errors while loading)
        /// </summary>
        public event Action<EndLoadHandler> OnEndLoading;
        /// <summary>
        /// Show current loading progress. Its changed from 0 to 1.
        /// </summary>
        public float CurrentProgress { get; private set; }
#if UNITY_EDITOR
        internal event Action EditorOnStartLoading;
#endif
        
        internal EventSystem() { }

        internal void BroadcastErrorMessage(LoadingError error)
        {
            OnError?.Invoke(error);
        }

        internal void BroadcastEndOfLoading(bool succes)
        {
            OnEndLoading?.Invoke(succes);
        }

        internal void UpdateLoadingProgress(float progress)
        {
            CurrentProgress = progress;
            OnLoadingProgress?.Invoke(progress);
        }

        internal void BroadcastStartLoading()
        {
#if UNITY_EDITOR
            EditorOnStartLoading?.Invoke();
#endif
            CurrentProgress = 0;
        }
    }
}