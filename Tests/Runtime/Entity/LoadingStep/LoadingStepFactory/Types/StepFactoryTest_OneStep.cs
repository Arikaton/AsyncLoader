using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LoadingModule.Contracts;
using LoadingModule.Editor.Entity;
using LoadingModule.Entity;
using UnityEngine;

namespace LoadingModule.Tests.Entity.Utils.Factories
{
    [LoadingStepFactoryIsHidden]
    public sealed class StepFactoryTest_OneStep : AbstractLoadingStepFactory
    {
        public override List<LoadingStep> CreateLoadingSteps()
        {
            return new List<LoadingStep>()
            {
                new Load_Artifact_1_noDependency()
            };
        }

        #region Loading Artifacts
        public interface ILoadingArtifact_1 : ILoadingArtifact { }
        public class LoadingArtifact_1 : ILoadingArtifact_1 { }
        #endregion

        #region Loading Steps
        private sealed class Load_Artifact_1_noDependency : LoadingStep
        {
            public Load_Artifact_1_noDependency() : base(typeof(ILoadingArtifact_1))
            {
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_1();
            }
        }
        #endregion
    }
}