using System.Collections.Generic;
using LoadingModule.Contracts;
using LoadingModule.Editor.Entity;
using LoadingModule.Entity;

namespace LoadingModule.Tests.Entity.Utils.Factories
{
    [LoadingStepFactoryIsHidden]
    [LoadingStepFactoryAllowEmptySteps]
    public sealed class StepFactoryTest_Empty : AbstractLoadingStepFactory
    {
        public override List<LoadingStep> CreateLoadingSteps()
        {
            return new List<LoadingStep>();
        }
    }
}