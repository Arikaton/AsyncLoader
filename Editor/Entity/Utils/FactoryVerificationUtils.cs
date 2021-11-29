using LoadingModule.Contracts;
using System;
using System.Collections.Generic;
using LoadingModule.Entity;
using UnityEditor;

namespace LoadingModule.Editor.Entity.Utils
{
    internal static class FactoryVerificationUtils
    {

        [InitializeOnLoadMethod]
        private static void Update()
        {
            try
            {
                CreateLoadingSteps();
                CreatedLoadingStepsMustReturnNotNull();
                CreatedLoadingStepsMustReturnNotEmptyList();
                CreatedLoadingStepsMustReturnListWithNoNullSteps();
                CreatedLoadingStepsMustHaveNotNullArtifactType();
                CreatedLoadingStepsDependencyMustBeResolved();
            }
            catch (Exception exp)
            {
                throw new Exception($"{Constants.LoadingModuleTag} [FactoryVerificationUtils] {exp.Message}");
            }

        }

        private static void CreateLoadingSteps()
        {
            var factories = FindUtils.GetVisibleFactoryInstances();
            factories?.ForEach(factory => factory.CreateLoadingSteps());
        }

        private static void CreatedLoadingStepsMustReturnNotNull()
        {
            var factories = FindUtils.GetVisibleFactoryInstances();

            if (factories == null)
                return;

            foreach (var factory in factories)
            {
                var steps = factory.CreateLoadingSteps();

                if (steps == null)
                    throw new Exception($"{Constants.LoadingModuleTag} Factory <{factory}> has created null list of steps");
            }
        }


        private static void CreatedLoadingStepsMustReturnNotEmptyList()
        {
            var factories = FindUtils.GetVisibleFactoryInstances();

            if (factories == null)
                return;

            foreach (var factory in factories)
            {
                var steps = factory.CreateLoadingSteps();

                if (steps.Count == 0)
                    throw new Exception($"{Constants.LoadingModuleTag} Factory <{factory}> has created empty list of steps");
            }
        }

        private static void CreatedLoadingStepsMustReturnListWithNoNullSteps()
        {
            var factories = FindUtils.GetVisibleFactoryInstances();

            if (factories == null)
                return;

            foreach (var factory in factories)
            {
                var steps = factory.CreateLoadingSteps();

                for (int i = 0; i < steps.Count; i++)
                {
                    if (steps[i] == null)
                        throw new Exception($"{Constants.LoadingModuleTag} Factory <{factory}> has created list with null step <{steps[i]}>");
                }
            }
        }

        private static void CreatedLoadingStepsMustHaveNotNullArtifactType()
        {
            var factories = FindUtils.GetVisibleFactoryInstances();

            if (factories == null)
                return;

            foreach (var factory in factories)
            {
                try
                {
                    VerifyLoadingStepsHaveNotNullArtifactType(factory);
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }

        }

        private static void VerifyLoadingStepsHaveNotNullArtifactType(AbstractLoadingStepFactory factory)
        {
            var steps = factory.CreateLoadingSteps();

            foreach (var step in steps)
            {
                if (step.ArtifactType == null)
                    throw new Exception($"{Constants.LoadingModuleTag} Factory <{factory}> has created list with null ArtifactType in step <{step}>");
            }
        }


        private static void CreatedLoadingStepsDependencyMustBeResolved()
        {
            var factories = FindUtils.GetVisibleFactoryInstances();

            if (factories == null)
                return;

            foreach (var factory in factories)
            {
                try
                {
                    VerifyLoadingStepsDependencyCanBeResolved(factory);
                }
                catch (Exception exp)
                {
                    throw exp;
                }
            }
        }

        private static void VerifyLoadingStepsDependencyCanBeResolved(AbstractLoadingStepFactory factory)
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
                    if (!stepsExpectedArtifactsTypeSet.Contains(dependency.ArtifactType))
                        throw new Exception($"{Constants.LoadingModuleTag} Factory <{factory}> has created list with step <{step}> with dependency on <{dependency}> that can't be resolved");
                }
            }
        }
    }
}
