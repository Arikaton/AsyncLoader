using System;
using System.Collections.Generic;
using System.Threading;
using LoadingModule.Contracts;
using Cysharp.Threading.Tasks;
using System.ComponentModel;

namespace LoadingModule.Entity
{
    public abstract class LoadingStep
    {
        public event Action<LoadingStatus> OnChangeLoadingStatus;
        public Type ArtifactType { get; }
        public IReadOnlyList<IDependency> Dependencies => _dependencies;
        internal bool HasDependencies => Dependencies.Count != 0;
        internal LoadingStatus LoadingStatus {
            get => _loadingStatus;
            private set
            {
                _loadingStatus = value;
                OnChangeLoadingStatus?.Invoke(value);
            }
        }

        protected readonly List<IDependency> _dependencies;
        private LoadingStatus _loadingStatus;
        private ILoadingArtifact _artifact;

        protected LoadingStep(Type artifactType)
        {
            if (artifactType == null)
                throw new NullReferenceException();

            if (!typeof(ILoadingArtifact).IsAssignableFrom(artifactType))
                throw new ArgumentException();


            ArtifactType = artifactType;
            LoadingStatus = LoadingStatus.NotLoaded;
            _dependencies = new List<IDependency>();
            _artifact = null;
        }

        internal async UniTask<ILoadingArtifact> LoadInternal()
        {
            switch (LoadingStatus)
            {
                case LoadingStatus.NotLoaded:   
                    return await TryLoad();
                case LoadingStatus.Aborted:     
                    return null;
                case LoadingStatus.Loading:     
                    throw new ThreadInterruptedException($"{Constants.LoadingModuleTag} LoadingStep <{GetType()}> already have been loading");
                case LoadingStatus.Loaded:
                    {
                        if (_artifact == null)
                            throw new NullReferenceException($"{Constants.LoadingModuleTag} LoadingStep <{GetType()}> have loaded status, but don't have artifact");

                        return _artifact;
                    }
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        internal void Abort()
        {
            if (LoadingStatus != LoadingStatus.Loaded)
                LoadingStatus = LoadingStatus.Aborted;
        }

        internal void ChangeAbortedLoadingStatusToNotLoaded()
        {
            if (LoadingStatus == LoadingStatus.Aborted)
                LoadingStatus = LoadingStatus.NotLoaded;
        }

        public override string ToString()
        {
            return GetType().Name + " " + ArtifactType.Name;
        }

        /// <summary>
        /// Contain business logic for loading step
        /// </summary>
        /// <returns>Return Artifact</returns>
        protected abstract UniTask<ILoadingArtifact> Load();

        private async UniTask<ILoadingArtifact> TryLoad()
        {
            ILoadingArtifact result;
            LoadingStatus = LoadingStatus.Loading;

            try
            {
                result = await Load();
            }
            catch (Exception exp)
            {
                LoadingStatus = LoadingStatus.Aborted;
                throw exp;
            }

            if (result == null)
                throw new NullReferenceException();

            //TODO: Probably delete this line
            if (LoadingStatus == LoadingStatus.Aborted)
                return null;

            _artifact = result;
            LoadingStatus = LoadingStatus.Loaded;
            return result;
        }
    }
}