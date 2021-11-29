using System;
using LoadingModule.Entity.Cache;
using LoadingModule.Contracts;

namespace LoadingModule.Entity
{
    public static class LoadingInitializer
    {
#if UNITY_EDITOR
        internal static event Action<LoadingController> OnInitFinished;
#endif
        
        private static LoadingStepFactoryCache _loadingStepFactoryCache = new LoadingStepFactoryCache();

        /// <summary>
        /// Initialize LoadingModule with defined step factory.
        /// </summary>
        /// <param name="stepFactory"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public static LoadingController Init(AbstractLoadingStepFactory stepFactory)
        {
            if (stepFactory == null)
                throw new NullReferenceException();
            
            _loadingStepFactoryCache.Add(stepFactory);

            var steps = stepFactory.CreateLoadingSteps();
            var nullStepsCount = 0;
            for (var i = 0; i < steps.Count; i++)
            {
                if (steps[i] is null)
                {
                    nullStepsCount++;
                }
            }

            if (nullStepsCount > 0)
                throw new Exception($"{Constants.LoadingModuleTag} There is {nullStepsCount} null steps. " +
                                    $"Fix your {stepFactory.GetType().Name} step factory.");

            var eventSystem = new EventSystem();
            var graphData = GraphUtils.BuildGraph(steps);
            var loadingController = new LoadingController(graphData, eventSystem);
#if UNITY_EDITOR
            OnInitFinished?.Invoke(loadingController);
            OnInitFinished = null;
#endif
            return loadingController;
        }

        internal static void ClearFactoryCache()
        {
            _loadingStepFactoryCache.Clear();
        }
    }
}