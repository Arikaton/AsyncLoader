using NUnit.Framework;
using LoadingModule.Contracts;
using LoadingModule.Entity;
using LoadingModule.Tests.Entity.Utils;
using LoadingModule.Editor.Entity;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace LoadingModule.Tests.Editor.Entity
{
    internal sealed class LoadingInitializerTest
    {
        #region Classes For Tests
        [LoadingStepAllowException]
        sealed class LoadingStep1 : LoadingStep
        {
            public LoadingStep1() : base(typeof(ILoadingArtifact)) { }

            protected override UniTask<ILoadingArtifact> Load()
            {
                throw new NotImplementedException();
            }
        }

        [LoadingStepAllowException]
        sealed class LoadingStep2 : LoadingStep
        {
            public LoadingStep2() : base(typeof(ILoadingArtifact)) { }

            protected override UniTask<ILoadingArtifact> Load()
            {
                throw new NotImplementedException();
            }
        }

        [LoadingStepFactoryNotUsable]
        class Factory1 : AbstractLoadingStepFactory
        {
            public override List<LoadingStep> CreateLoadingSteps()
            {
                return new List<LoadingStep>()
                {
                    new LoadingStep1(),
                    new LoadingStep2()
                };
            }
        }

        [LoadingStepFactoryNotUsable]
        class Factory2 : AbstractLoadingStepFactory
        {
            public override List<LoadingStep> CreateLoadingSteps()
            {
                return new List<LoadingStep>()
                {
                    new LoadingStep1(),
                    new LoadingStep2()
                };
            }
        }
        #endregion

        [Test]
        public void InitAnyFactoryThrowsNoException()
        {
            var factories =  LoadingStepFactoryTest.GetAllFactoryWithoutEmptyStepList();
            LoadingInitializer.ClearFactoryCache();

            Assert.DoesNotThrow(() =>
            {
                foreach (var factory in factories)
                {
                    LoadingInitializer.Init(factory);
                }
            });
        }

        [Test]
        public void InitWithNullThrowsException()
        {
            LoadingInitializer.ClearFactoryCache();

            Assert.Throws<NullReferenceException>(() =>
            {
                LoadingInitializer.Init(null);
            });
        }

        [Test]
        public void InitAnyFactorySecondTimeThrowsException()
        {
            var factories = LoadingStepFactoryTest.GetAllFactoryWithoutEmptyStepList();
            LoadingInitializer.ClearFactoryCache();

            foreach (var factory in factories)
            {
                LoadingInitializer.Init(factory);
            }

            foreach (var factory in factories)
            {
                Assert.Throws<ArgumentException>(() =>
                {
                    LoadingInitializer.Init(factory);
                });
            }
        }

        [Test]
        public void InitSpecificFactorySecondTimeThrowsException()
        {
            var factory = new Factory1();
            LoadingInitializer.ClearFactoryCache();

            LoadingInitializer.Init(factory);

            Assert.Throws<ArgumentException>(() =>
            {
                LoadingInitializer.Init(factory);
            });
        }

        [Test]
        public void InitDifferentFactoriesWithSameStepsThrowsNoException()
        {
            LoadingInitializer.ClearFactoryCache();

            LoadingInitializer.Init(new Factory1());

            Assert.DoesNotThrow(() =>
            {
                LoadingInitializer.Init(new Factory2());
            });
        }
    }
}
