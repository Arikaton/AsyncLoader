using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LoadingModule.Contracts;
using LoadingModule.Entity;
using LoadingModule.Editor.Entity;
using UnityEngine;

namespace LoadingModule.Tests.Entity.Utils.Factories
{
    [LoadingStepFactoryIsHidden]
    public sealed class StepFactoryTest_Cycle : AbstractLoadingStepFactory
    {
        public override List<LoadingStep> CreateLoadingSteps()
        {
            return new List<LoadingStep>()
            {
                new Load_Artifact_1_noDepndency(),
                new Load_Artifact_2_noDependency(),
                new Load_Artifact_3_withDependency_on_1_and_2(),
                new Load_Artifact_4_withDependency_on_3(),
                new Load_Artifact_5_withDependency_on_3(),
                new Load_Artifact_6_withDependency_on_5_and_7(),
                new Load_Artefact_7_withDependency_on_6()
            };
        }

        #region Loading Artifacts
        public interface ILoadingArtifact_1 : ILoadingArtifact { }
        public interface ILoadingArtifact_2 : ILoadingArtifact { }
        public interface ILoadingArtifact_3 : ILoadingArtifact_1, ILoadingArtifact_2 { }
        public interface ILoadingArtifact_4 : ILoadingArtifact_3 { }
        public interface ILoadingArtifact_5 : ILoadingArtifact_3 { }
        public interface ILoadingArtifact_6 : ILoadingArtifact_5 { }
        public interface ILoadingArtifact_7 : ILoadingArtifact_6 { }

        public class LoadingArtifact_1 : ILoadingArtifact_1 { }
        public class LoadingArtifact_2 : ILoadingArtifact_2 { }
        public class LoadingArtifact_3 : ILoadingArtifact_3 { }
        public class LoadingArtifact_4 : ILoadingArtifact_4 { }
        public class LoadingArtifact_5 : ILoadingArtifact_5 { }
        public class LoadingArtifact_6 : ILoadingArtifact_6 { }
        public class LoadingArtifact_7 : ILoadingArtifact_7 { }
        #endregion

        #region Loading Steps
        private sealed class Load_Artifact_1_noDepndency : LoadingStep
        {
            public Load_Artifact_1_noDepndency() : base(typeof(ILoadingArtifact_1))
            {
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_1();
            }
        }

        private sealed class Load_Artifact_2_noDependency : LoadingStep
        {
            public Load_Artifact_2_noDependency() : base(typeof(ILoadingArtifact_2))
            {
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_2();
            }
        }

        private sealed class Load_Artifact_3_withDependency_on_1_and_2 : LoadingStep
        {
            public Load_Artifact_3_withDependency_on_1_and_2() : base(typeof(ILoadingArtifact_3))
            {
                _dependencies.Add(new Dependency<ILoadingArtifact_1>(x => Debug.Log("Artifact1 was downloaded")));
                _dependencies.Add(new Dependency<ILoadingArtifact_2>(x => Debug.Log("Artifact2 was downloaded")));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_3();
            }
        }

        private sealed class Load_Artifact_4_withDependency_on_3 : LoadingStep
        {
            public Load_Artifact_4_withDependency_on_3() : base(typeof(ILoadingArtifact_4))
            {
                _dependencies.Add(new Dependency<ILoadingArtifact_3>(x => Debug.Log("Artifact3 loaded")));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_4();
            }
        }

        private sealed class Load_Artifact_5_withDependency_on_3 : LoadingStep
        {
            public Load_Artifact_5_withDependency_on_3() : base(typeof(ILoadingArtifact_5))
            {
                _dependencies.Add(new Dependency<ILoadingArtifact_3>(x => Debug.Log("Artifact3 loaded")));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_5();
            }
        }

        private sealed class Load_Artifact_6_withDependency_on_5_and_7 : LoadingStep
        {
            public Load_Artifact_6_withDependency_on_5_and_7() : base(typeof(ILoadingArtifact_6))
            {
                _dependencies.Add(new Dependency<ILoadingArtifact_5>(x => Debug.Log("Artifact5 loaded")));
                _dependencies.Add(new Dependency<ILoadingArtifact_7>(x => Debug.Log("Artifact7 loaded")));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_6();
            }
        }

        private sealed class Load_Artefact_7_withDependency_on_6 : LoadingStep
        {
            public Load_Artefact_7_withDependency_on_6() : base(typeof(ILoadingArtifact_7))
            {
                _dependencies.Add(new Dependency<ILoadingArtifact_6>(x => Debug.Log("Artifact6 loaded")));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(5);
                return new LoadingArtifact_7();
            }
        }
        #endregion
    }
}
