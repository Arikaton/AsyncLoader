using NUnit.Framework;
using System;
using LoadingModule.Contracts;
using LoadingModule.Tests.Entity.Utils;

namespace LoadingModule.Tests.Editor.Entity.Step
{
    internal sealed class LoadingStepTest
    {
        [Test]
        public void ConstructorArtifactIsNullThrowsException()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                var loadingStep = new LoadingStepModel.LoadingStepWithFullConstructor(null);
            });
        }

        [TestCase(typeof(int))]
        [TestCase(typeof(bool))]
        [TestCase(typeof(object))]
        [TestCase(typeof(string))]
        public void ConstructorArtifactIsNotArtifactTypeThrowsException(Type artifactType)
        {
            Assert.Throws<ArgumentException>(() =>
           {
               var loadingStep = new LoadingStepModel.LoadingStepWithFullConstructor(artifactType);
           });
        }

        [Test]
        public void ConstructorArtifactIsArtifactTypeThrowsNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var loadingStep = new LoadingStepModel.LoadingStepWithFullConstructor(typeof(ILoadingArtifact));
            });
        }
    }
}
