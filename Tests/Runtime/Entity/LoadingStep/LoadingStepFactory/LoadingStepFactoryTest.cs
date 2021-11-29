using LoadingModule.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;
using Cysharp.Threading.Tasks;
using System.Collections;
using LoadingModule.Editor.Entity.Utils;

namespace LoadingModule.Tests.Entity.Utils
{
    public class LoadingStepFactoryTest
    {
        [UnityTest]
        public IEnumerator LoadingStepsLoadThrowsNoException() => UniTask.ToCoroutine(async () =>
        {
            var factories = GetAllFactoryWithoutEmptyStepList();

            foreach (var factory in factories)
            {
                await VerifyLoadingStepsLoadThrowsNoException(factory);
            }
        });

        internal async UniTask VerifyLoadingStepsLoadThrowsNoException(AbstractLoadingStepFactory factory)
        {
            var steps = factory.CreateLoadingSteps();

            foreach (var step in steps)
            {
                await RuntimeTestUtils.VerifyLoadInternalThrowsNoException(step);
            }
        }



        [UnityTest]
        public IEnumerator LoadingStepsLoadReturnsNotNull() => UniTask.ToCoroutine(async () =>
        {
            var factories = GetAllFactoryWithoutEmptyStepList();

            foreach (var factory in factories)
            {
                await VerifyLoadingStepsLoadReturnsNotNull(factory);
            }
        });

        internal async UniTask VerifyLoadingStepsLoadReturnsNotNull(AbstractLoadingStepFactory factory)
        {
            var steps = factory.CreateLoadingSteps();

            foreach (var step in steps)
            {
                await RuntimeTestUtils.VerifyLoadInternalReturnsNotNull(step);
            }
        }

        [UnityTest]
        public IEnumerator LoadingStepsLoadReturnsSameTypeAsArtifactType() => UniTask.ToCoroutine(async () =>
       {
           var factories = GetAllFactoryWithoutEmptyStepList();

           foreach (var factory in factories)
           {
               await VerifyLoadingStepsLoadReturnsSameTypeAsArtifactType(factory);
           }
       });

        internal async UniTask VerifyLoadingStepsLoadReturnsSameTypeAsArtifactType(AbstractLoadingStepFactory factory)
        {
            var steps = factory.CreateLoadingSteps();

            foreach (var step in steps)
            {
                await RuntimeTestUtils.VerifyLoadInternalReturnsSameTypeAsArtifactType(step);
            }
        }

        [UnityTest]
        public IEnumerator LoadingStepsLoadReturnsUniqueElements() => UniTask.ToCoroutine(async () =>
        {
            var factories = GetAllFactoryWithoutEmptyStepList();

            foreach (var factory in factories)
            {
                await VerifyLoadingStepsLoadReturnsUniqueElements(factory);
            }
        });

        internal async UniTask VerifyLoadingStepsLoadReturnsUniqueElements(AbstractLoadingStepFactory factory)
        {
            var steps = factory.CreateLoadingSteps();
            var stepsExpectedArtifactsTypeSet = new HashSet<Type>();
            var stepsActualArtifactsTypeSet = new HashSet<Type>();

            foreach (var step in steps)
            {
                Assert.IsFalse(stepsExpectedArtifactsTypeSet.Contains(step.ArtifactType));
                stepsExpectedArtifactsTypeSet.Add(step.ArtifactType);

                var artifact = await RuntimeTestUtils.GetLoadInternalArtifact(step);

                if (artifact != null)
                {
                    Assert.IsFalse(stepsActualArtifactsTypeSet.Contains(artifact.GetType()));
                    stepsActualArtifactsTypeSet.Add(artifact.GetType());
                }
            }
        }

        internal static List<AbstractLoadingStepFactory> GetAllFactoryWithoutEmptyStepList()
        {
            return FindUtils.GetAllFactoryInstances()
                .Where(f =>
                {
                    return Attribute.GetCustomAttribute(f.GetType(), typeof(LoadingStepFactoryAllowEmptyStepsAttribute)) == null;
                })
                .ToList();
        }
    }
}
