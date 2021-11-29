using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LoadingModule.Contracts;
using LoadingModule.Editor.Entity;
using LoadingModule.Entity;
using UnityEngine;

namespace LoadingModule.Tests.Entity.Utils.Factories
{
    [LoadingStepFactoryIsHidden]
    public sealed class StepFactoryTest_Good : AbstractLoadingStepFactory
    {
        private readonly int _stepsDelay;

        public StepFactoryTest_Good()
        {
            _stepsDelay = 5;
        }

        public StepFactoryTest_Good(int stepsDelay)
        {
            _stepsDelay = stepsDelay;
        }


        public override List<LoadingStep> CreateLoadingSteps()
        {
            return new List<LoadingStep>()
            {
                new Load_Artifact_1_noDepndency(_stepsDelay),
                new Load_Artifact_2_noDependency(_stepsDelay),
                new Load_Artifact_3_withDependency_on_1_and_2(_stepsDelay),
                new Load_Artifact_4_withDependency_on_3(_stepsDelay),
                new Load_Artifact_5_withDependency_on_3(_stepsDelay),
                new Load_Artifact_6_withDependency_on_5(_stepsDelay)
            };
        }

        #region Loading Artifacts
        public interface ILoadingArtifact_1 : ILoadingArtifact { }
        public interface ILoadingArtifact_2 : ILoadingArtifact { }
        public interface ILoadingArtifact_3 : ILoadingArtifact_1, ILoadingArtifact_2 { }
        public interface ILoadingArtifact_4 : ILoadingArtifact_3 { }
        public interface ILoadingArtifact_5 : ILoadingArtifact_3 { }
        public interface ILoadingArtifact_6 : ILoadingArtifact_5 { }

        public class LoadingArtifact_1 : ILoadingArtifact_1 { }
        public class LoadingArtifact_2 : ILoadingArtifact_2 { }
        public class LoadingArtifact_3 : ILoadingArtifact_3 { }
        public class LoadingArtifact_4 : ILoadingArtifact_4 { }
        public class LoadingArtifact_5 : ILoadingArtifact_5 { }
        public class LoadingArtifact_6 : ILoadingArtifact_6 { }
        #endregion

        #region Loading Steps
        private sealed class Load_Artifact_1_noDepndency : LoadingStep
        {
            private readonly int _delay;

            public Load_Artifact_1_noDepndency(int delay) : base(typeof(ILoadingArtifact_1))
            {
                _delay = delay;
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(_delay);
                Debug.Log("Artifact 1 has been loaded");
                return new LoadingArtifact_1();
            }
        }

        private sealed class Load_Artifact_2_noDependency : LoadingStep
        {
            private readonly int _delay;

            public Load_Artifact_2_noDependency(int delay) : base(typeof(ILoadingArtifact_2))
            {
                _delay = delay;
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(_delay);
                Debug.Log("Artifact 2 has been loaded");
                return new LoadingArtifact_2();
            }
        }

        private sealed class Load_Artifact_3_withDependency_on_1_and_2 : LoadingStep
        {
            private readonly int _delay;

            public Load_Artifact_3_withDependency_on_1_and_2(int delay) : base(typeof(ILoadingArtifact_3))
            {
                _delay = delay;
                _dependencies.Add(new Dependency<ILoadingArtifact_1>(null));
                _dependencies.Add(new Dependency<ILoadingArtifact_2>(null));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(_delay);
                Debug.Log("Artifact 3 has been loaded");
                return new LoadingArtifact_3();
            }
        }

        private sealed class Load_Artifact_4_withDependency_on_3 : LoadingStep
        {
            private readonly int _delay;

            public Load_Artifact_4_withDependency_on_3(int delay) : base(typeof(ILoadingArtifact_4))
            {
                _delay = delay;
                _dependencies.Add(new Dependency<ILoadingArtifact_3>(null));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(_delay);
                Debug.Log("Artifact 4 has been loaded");
                return new LoadingArtifact_4();
            }
        }

        private sealed class Load_Artifact_5_withDependency_on_3 : LoadingStep
        {
            private readonly int _delay;

            public Load_Artifact_5_withDependency_on_3(int delay) : base(typeof(ILoadingArtifact_5))
            {
                _delay = delay;
                _dependencies.Add(new Dependency<ILoadingArtifact_3>(null));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(_delay);
                Debug.Log("Artifact 5 has been loaded");
                return new LoadingArtifact_5();
            }
        }

        private sealed class Load_Artifact_6_withDependency_on_5 : LoadingStep
        {
            private readonly int _delay;

            public Load_Artifact_6_withDependency_on_5(int delay) : base(typeof(ILoadingArtifact_6))
            {
                _delay = delay;
                _dependencies.Add(new Dependency<ILoadingArtifact_5>(null));
            }

            protected override async UniTask<ILoadingArtifact> Load()
            {
                await UniTask.Delay(_delay);
                Debug.Log("Artifact 6 has been loaded");
                return new LoadingArtifact_6();
            }
        }
        #endregion
    }
}