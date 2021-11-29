using System;

namespace LoadingModule.Contracts
{
    public interface IDependency
    {
        bool Resolve(ILoadingArtifact artifact);
        Type ArtifactType { get; }
    }
}