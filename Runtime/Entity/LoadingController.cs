using System;

namespace LoadingModule.Entity
{
    public sealed class LoadingController
    {
        /// <summary>
        /// Used for loading module events.
        /// </summary>
        public EventSystem EventSystem { get; }
        /// <summary>
        /// Shows the current loading status.
        /// </summary>
        public bool IsLoading { get; private set; }
        internal event Action OnAborted;
        internal GraphData GraphData { get; }
        internal int TotalNodesCount => GraphData.NodesCount;
        internal int LoadedNodesCount { get; private set; }
        internal int LoadingNodesCount { get; private set; }
        
        private bool _wasLoadedOnce;
        private bool _isAborted;
        private bool _successLoading;

        internal LoadingController(GraphData graphData, EventSystem eventSystem)
        {
            GraphData = graphData ?? throw new NullReferenceException();
            EventSystem = eventSystem ?? throw new NullReferenceException();

            foreach (var node in GraphData.Nodes)
            {
                node.OnStartLoading += OnNodeStartLoading;
                node.OnEndLoading += OnNodeEndLoading;
                node.OnError += OnNodeError;
            }
        }
        
        /// <summary>
        /// Starts loading.
        /// </summary>
        public void Load()
        {
            if (IsLoading)
                return;
            
            if (!_wasLoadedOnce)
                _wasLoadedOnce = true;
            else
                PrepareToReload();

            _successLoading = true;
            EventSystem.BroadcastStartLoading(); 

            IsLoading = true;

            foreach (var node in GraphData.RootNode.NextNodes)
            {
                node?.Load();
            }
        }

        private void PrepareToReload()
        {
            _isAborted = false;
            foreach (var graphNode in GraphData.Nodes)
            {
                graphNode.Reset();
                graphNode.Step.ChangeAbortedLoadingStatusToNotLoaded();
            }
        }
        
        /// <summary>
        /// Abort all steps and stop loading.
        /// </summary>
        public void Abort()
        {
            if (_isAborted) return;
            
            _isAborted = true;
            _successLoading = false;
            foreach (var graphNode in GraphData.Nodes)
            {
                graphNode.Abort();
            }

            OnAborted?.Invoke();
        }

        private void OnNodeStartLoading(GraphNode node)
        {
            ++LoadingNodesCount;
        }

        private void OnNodeEndLoading(GraphNode node)
        {
            --LoadingNodesCount;
            ++LoadedNodesCount;
            EventSystem.UpdateLoadingProgress((float)LoadedNodesCount / TotalNodesCount);

            if(LoadedNodesCount == TotalNodesCount)
            {
                IsLoading = false;
                EventSystem.BroadcastEndOfLoading(_successLoading);
            }
        }

        private void OnNodeError(GraphNode node, LoadingError message)
        {
            _successLoading = false;
            EventSystem.BroadcastErrorMessage(message);
        }
    }
}