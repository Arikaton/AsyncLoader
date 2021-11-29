using System;
using LoadingModule.Contracts;

namespace LoadingModule.Entity
{
    public sealed class Dependency<T> : IDependency where T : ILoadingArtifact
    {
        private readonly Action<T> _callback;
        public Type ArtifactType { get; }
        public Dependency(Action<T> callback)
        {
            _callback = callback;
            ArtifactType = typeof(T);
        }

        bool IDependency.Resolve(ILoadingArtifact artifact)
        {
            if (artifact is T typedArtifact)
            {
                _callback?.Invoke(typedArtifact);
                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return ArtifactType.Name;
        }
    }
}