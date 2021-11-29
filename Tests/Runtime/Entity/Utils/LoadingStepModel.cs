using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LoadingModule.Contracts;
using LoadingModule.Entity;
using UnityEngine;
using Moq;
using Moq.Protected;

namespace LoadingModule.Tests.Entity.Utils
{
    public class LoadingStepModel
    {
        public class LoadingStepWithFullConstructor : LoadingStep
        {
            private readonly Func<ILoadingArtifact> _load;

            public LoadingStepWithFullConstructor(Type artifactType, List<IDependency> dependencies = null, Func<ILoadingArtifact> load = null) : base(artifactType)
            {
                if (dependencies != null && dependencies.Count > 0)
                {
                    _dependencies.AddRange(dependencies);
                }

                _load = load ?? null;
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                var artifact = _load?.Invoke();
                Debug.Log($"{{Loading Step with Artifact Type and Dependency and Load}} loaded artifact |{ArtifactType}|");
                return artifact;
            }
        }

        public static Mock<LoadingStep> CreateLoadingStepMock(int delay)
        {
            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));

            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(async () =>
            {
                Debug.Log("Loading Starts");
                await UniTask.Delay(delay);
                Debug.Log("Loading Ends");
                return new Mock<ILoadingArtifact>().Object;
            });

            return loadingStepMock;
        }

        public static Mock<LoadingStep> CreateLoadingStepExceptionMock()
        {
            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));

            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(async () =>
            {
                throw new Exception();
            });

            return loadingStepMock;
        }
    }
}
