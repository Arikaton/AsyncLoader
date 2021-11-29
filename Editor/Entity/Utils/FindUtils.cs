using LoadingModule.Contracts;
using LoadingModule.Entity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LoadingModule.Editor.Entity.Utils
{
    internal static class FindUtils
    {
        private static IEnumerable<Type> GetStepTypes()
        {
            var stepTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(LoadingStep)) && !t.IsAbstract);

            return stepTypes;
        }

        internal static List<string> GetStepNames()
        {
            var stepNames = GetStepTypes()
                .Select(x => x.Name.Split('.').Last())
                .ToList();

            return stepNames;
        }

        private static IEnumerable<Type> GetAllFactoryTypes()
        {
            var factories = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(AbstractLoadingStepFactory).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .Where(t =>
                {
                    return Attribute.GetCustomAttribute(t, typeof(LoadingStepFactoryNotUsableAttribute)) == null;
                });
            
            return factories;
        }

        private static IEnumerable<Type> GetVisibleFactoryTypes()
        {
            var factories = GetAllFactoryTypes()
                .Where(t =>
                {
                    return Attribute.GetCustomAttribute(t, typeof(LoadingStepFactoryIsHiddenAttribute)) == null;
                });

            return factories;
        }

        internal static List<AbstractLoadingStepFactory> GetVisibleFactoryInstances()
        {
            var factories = GetVisibleFactoryTypes()
                .Select(x => (AbstractLoadingStepFactory)Activator.CreateInstance(x))
                .ToList();

            return factories;
        }

        internal static List<string> GetVisibleFactoryNames()
        {
            var factoriesNames = GetVisibleFactoryTypes()
                .Select(x => x.Name)
                .ToList();

            return factoriesNames;
        }

        internal static List<AbstractLoadingStepFactory> GetAllFactoryInstances()
        {
            var factories = GetAllFactoryTypes()
                .Select(x => (AbstractLoadingStepFactory)Activator.CreateInstance(x))
                .ToList();

            return factories;
        }

        internal static List<string> GetAllFactoryNames()
        {
            var factoriesNames = GetAllFactoryTypes()
                .Select(x => x.Name)
                .ToList();

            return factoriesNames;
        }
    }
}
