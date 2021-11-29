using System.Collections.Generic;
using LoadingModule.Entity;

namespace LoadingModule.Contracts
{
    public abstract class AbstractLoadingStepFactory
    {
        public abstract List<LoadingStep> CreateLoadingSteps();

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode();
        }
    }
}