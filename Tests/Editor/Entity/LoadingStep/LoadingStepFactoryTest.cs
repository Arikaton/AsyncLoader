using System;
using NUnit.Framework;
using LoadingModule.Editor.Entity.Utils;
using LoadingModule.Tests.Editor.Entity.Utils;
using LoadingModule.Tests.Entity.Utils;

namespace LoadingModule.Tests.Editor.Entity.Step
{
    internal sealed class LoadingStepFactoryTest
    {
        [Test]
        public void EachFactoryNotNull()
        {
            var factories = FindUtils.GetAllFactoryInstances();

            foreach (var factory in factories)
            {
                Assert.NotNull(factory);
            }
        }

        [Test]
        public void CreateLoadingStepsThrowsNoException()
        {
            var factories = FindUtils.GetAllFactoryInstances();

            foreach (var factory in factories)
            {
                Assert.DoesNotThrow(() =>
                {
                    var steps = factory.CreateLoadingSteps();
                });
            }
        }

        [Test]
        public void CreateLoadingStepsReturnNotNull()
        {
            var factories = FindUtils.GetAllFactoryInstances();

            foreach (var factory in factories)
            {
                var steps = factory.CreateLoadingSteps();
                Assert.NotNull(steps);
            }
        }

        [Test]
        public void CreateLoadingStepsReturnNotEmptyList()
        {
            var factories = FindUtils.GetAllFactoryInstances();

            foreach (var factory in factories)
            {
                var steps = factory.CreateLoadingSteps();
                var attribute = Attribute.GetCustomAttribute(factory.GetType(), typeof(LoadingStepFactoryAllowEmptyStepsAttribute));

                if (attribute == null)
                {
                    Assert.NotZero(steps.Count);
                }
                else
                {
                    Assert.Zero(steps.Count);
                }
            }
        }

        [Test]
        public void CreateLoadingStepsReturnListWithNoEmptySteps()
        {
            var factories = Tests.Entity.Utils.LoadingStepFactoryTest.GetAllFactoryWithoutEmptyStepList();

            foreach (var factory in factories)
            {
                var steps = factory.CreateLoadingSteps();

                for (int i = 0; i < steps.Count; i++)
                {
                    Assert.NotNull(steps[i]);
                }
            }
        }

        [Test]
        public void LoadingStepsHaveNotNullArtifactType()
        {
            var factories = Tests.Entity.Utils.LoadingStepFactoryTest.GetAllFactoryWithoutEmptyStepList();

            foreach (var factory in factories)
            {
                EditorTestUtils.VerifyLoadingStepsHaveNotNullArtifactType(factory);
            }
        }

        [Test]
        public void LoadingStepsDependencyCanBeResolved()
        {
            var factories = Tests.Entity.Utils.LoadingStepFactoryTest.GetAllFactoryWithoutEmptyStepList();

            foreach (var factory in factories)
            {
                EditorTestUtils.VerifyLoadingStepsDependencyCanBeResolved(factory);
            }
        }

        
    }
}
