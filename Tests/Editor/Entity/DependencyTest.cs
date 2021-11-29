using NUnit.Framework;
using LoadingModule.Contracts;
using LoadingModule.Entity;

namespace LoadingModule.Tests.Editor.Entity
{
    internal sealed class DependencyTest
    {
        interface IArtifactBase : ILoadingArtifact { };
        interface IArtifactDerived : IArtifactBase { };
        class ArtifactBase : IArtifactBase { };
        class ArtifactDerived : IArtifactDerived { };

        [Test]
        public void ResolveSameArtifactThrowsNoException()
        {
            IDependency dependency = new Dependency<IArtifactBase>(null);
            var artifact = new ArtifactBase();

            Assert.DoesNotThrow(() =>
            {
                dependency.Resolve(artifact);
            });
        }

        [Test]
        public void ResolveBaseArtifactThrowsNoException()
        {
            IDependency dependency = new Dependency<IArtifactDerived>(null);
            var artifact = new ArtifactBase();

            Assert.DoesNotThrow(() =>
            {
                dependency.Resolve(artifact);
            });
        }

        [Test]
        public void ResolveDerivedArtifactThrowsNoException()
        {
            IDependency dependency = new Dependency<IArtifactBase>(null);
            var artifact = new ArtifactDerived();

            Assert.DoesNotThrow(() =>
            {
                dependency.Resolve(artifact);
            });
        }

        [Test]
        public void ResolveSameArtifactReturnsTrue()
        {
            IDependency dependency = new Dependency<IArtifactBase>(null);
            var artifact = new ArtifactBase();

            var result = dependency.Resolve(artifact);
            Assert.IsTrue(result);
        }


        [Test]
        public void ResolveBaseArtifactReturnsFalse()
        {
            IDependency dependency = new Dependency<IArtifactDerived>(null);
            var artifact = new ArtifactBase();

            var result = dependency.Resolve(artifact);
            Assert.IsFalse(result);
        }

        [Test]
        public void ResolveDerivedArtifactReturnsTrue()
        {
            IDependency dependency = new Dependency<IArtifactBase>(null);
            var artifact = new ArtifactDerived();

            var result = dependency.Resolve(artifact);
            Assert.IsTrue(result);
        }
    }
}
