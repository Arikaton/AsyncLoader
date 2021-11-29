using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LoadingModule.Contracts;
using UnityEngine;

namespace LoadingModule.Entity
{
    public sealed class GraphNode
    {
        internal event Action<GraphNode> OnEndLoading;
        internal event Action<GraphNode, LoadingError> OnError;
        internal event Action<GraphNode> OnStartLoading;
        internal GraphNodeState State { get; set; }
        internal LoadingStep Step { get; }
        internal IReadOnlyList<GraphNode> NextNodes { get; set; }
        private int resolvedDependencyCount;
        
        internal GraphNode(LoadingStep step)
        {
            State = GraphNodeState.NotChecked;
            Step = step;
        }
        
        internal async UniTask Load()
        {
            if (Step.LoadingStatus == LoadingStatus.Aborted) return;
            OnStartLoading?.Invoke(this);
            ILoadingArtifact loadingArtifact;
            
            try
            {
                loadingArtifact = await Step.LoadInternal();
            }
            catch (Exception e)
            {
                HandleError(e);
                return;
            }
            
            OnEndLoading?.Invoke(this);
            if (Step.LoadingStatus == LoadingStatus.Aborted) return;
            
            TryResolveNextNodes(loadingArtifact);
        }

        internal void Abort()
        {
            if (Step.LoadingStatus == LoadingStatus.Aborted)
                return;

            Step.Abort();
            OnEndLoading?.Invoke(this);
            AbortNextNodes();
            Reset();
        }

        internal void Reset()
        {
            resolvedDependencyCount = 0;
        }   
        
        #region private methods
        private void AbortNextNodes()
        {
            if (NextNodes == null) return;
            foreach (var nextNode in NextNodes)
            {
                nextNode.Abort();
            }
        }

        private void TryResolveNextNodes(ILoadingArtifact loadingArtifact)
        {
            if (NextNodes == null) return;
            foreach (var loadingGraphNode in NextNodes)
            {
                loadingGraphNode.Resolve(loadingArtifact);
            }
        }

        private void HandleError(Exception e)
        {
            Debug.LogWarning($"{Constants.LoadingModuleTag} {e.Message}");
            var error = new LoadingError(e.Message, e.StackTrace);
            OnError?.Invoke(this, error);
            OnEndLoading?.Invoke(this);
            AbortNextNodes();
        }

        private void Resolve(ILoadingArtifact loadedArtifact)
        {
            foreach (IDependency dependency in Step.Dependencies)
            {
                if (dependency.Resolve(loadedArtifact))
                {
                    resolvedDependencyCount++;
                }
            }
            
            if (resolvedDependencyCount == Step.Dependencies.Count)
            {
                Load();
            }
        }
        #endregion
    }
}