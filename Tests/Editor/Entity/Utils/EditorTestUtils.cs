using System;
using System.Collections.Generic;
using NUnit.Framework;
using LoadingModule.Contracts;

namespace LoadingModule.Tests.Editor.Entity.Utils
{
    public static class EditorTestUtils
    {
        internal static void VerifyLoadingStepsDependencyCanBeResolved(AbstractLoadingStepFactory factory)
        {
            var steps = factory.CreateLoadingSteps();
            var stepsExpectedArtifactsTypeSet = new HashSet<Type>();

            foreach (var step in steps)
            {
                stepsExpectedArtifactsTypeSet.Add(step.ArtifactType);
            }

            foreach (var step in steps)
            {
                foreach (var dependency in step.Dependencies)
                {
                    Assert.IsTrue(stepsExpectedArtifactsTypeSet.Contains(dependency.ArtifactType));
                }
            }
        }
        
        internal static void VerifyLoadingStepsHaveNotNullArtifactType(AbstractLoadingStepFactory factory)
        {
            var steps = factory.CreateLoadingSteps();

            foreach (var step in steps)
            {
                Assert.IsNotNull(step.ArtifactType);
            }
        }
    }
}