using System;
using System.Collections.Generic;
using LoadingModule.Contracts;

namespace LoadingModule.Entity.Cache
{
    internal class LoadingStepFactoryCache
    {
        private HashSet<AbstractLoadingStepFactory> cachedLoadingStepFactoriesTypes;

        internal LoadingStepFactoryCache()
        {
            Clear();
        }

        internal void Add(AbstractLoadingStepFactory factory)
        {
            if (factory == null)
                throw new NullReferenceException();

            if (cachedLoadingStepFactoriesTypes.Contains(factory))
                throw new ArgumentException($"{Constants.LoadingModuleTag} {factory} already cached");

            cachedLoadingStepFactoriesTypes.Add(factory);
        }

        internal bool Contains(AbstractLoadingStepFactory factory)
        {
            if (factory == null)
                throw new NullReferenceException();
            
            return cachedLoadingStepFactoriesTypes.Contains(factory);
        }

        internal void Clear()
        {
            cachedLoadingStepFactoriesTypes = new HashSet<AbstractLoadingStepFactory>();
        }
    }
}
