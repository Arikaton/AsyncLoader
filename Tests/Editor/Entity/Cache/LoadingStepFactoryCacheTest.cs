using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadingModule.Entity.Cache;
using LoadingModule.Tests;
using LoadingModule.Entity;
using LoadingModule.Contracts;
using LoadingModule.Editor.Entity;

namespace LoadingModule.Tests.Editor.Entity.Cache
{
    internal sealed class LoadingStepFactoryCacheTest
    {

        #region Help Classes
        [LoadingStepFactoryNotUsable]
        abstract class AbstractFactory : AbstractLoadingStepFactory
        {
            abstract public override List<LoadingStep> CreateLoadingSteps();
        }

        [LoadingStepFactoryNotUsable]
        class BaseFactory : AbstractLoadingStepFactory
        {
            public override List<LoadingStep> CreateLoadingSteps()
            {
                throw new NotImplementedException();
            }
        }
        [LoadingStepFactoryNotUsable]
        class DerivedFactory : BaseFactory { }
        #endregion

        #region Add
        [Test]
        public void AddNullThrowsException()
        {
            var cache = new LoadingStepFactoryCache();

            Assert.Throws<NullReferenceException>(() =>
            {
                cache.Add(null);
            });
        }

        [Test]
        public void AddFactoryThrowsNoException()
        {
            var cache = new LoadingStepFactoryCache();

            Assert.DoesNotThrow(() =>
            {
                cache.Add(new BaseFactory());
            });
        }

        [Test]
        public void AddSameFactorySecondTimeThrowsException()
        {
            var cache = new LoadingStepFactoryCache();
            cache.Add(new BaseFactory());

            Assert.Throws<ArgumentException>(() =>
            {
                cache.Add(new BaseFactory());
            });
        }

        [Test]
        public void AddBaseFactoryThenAddDerivedFactoryThrowsNoException()
        {
            var cache = new LoadingStepFactoryCache();

            Assert.DoesNotThrow(() =>
            {
                cache.Add(new BaseFactory());
                cache.Add(new DerivedFactory());
            });
        }

        [Test]
        public void AddDerivedFactoryThenAddBaseFactoryThrowsNoException()
        {
            var cache = new LoadingStepFactoryCache();

            Assert.DoesNotThrow(() =>
            {
                cache.Add(new DerivedFactory());
                cache.Add(new BaseFactory());
            });
        }
        #endregion

        #region Contains
        [Test]
        public void ContainsNullThrowsException()
        {
            var cache = new LoadingStepFactoryCache();

            Assert.Throws<NullReferenceException>(() =>
            {
                cache.Contains(null);
            });
        }

        [Test]
        public void ContainsNotCachedFactoryReturnsFalse()
        {
            var cache = new LoadingStepFactoryCache();
            var factory = new BaseFactory();
            var result = cache.Contains(factory);

            Assert.IsFalse(result);
        }

        [Test]
        public void ContainsCachedFactoryReturnsTrue()
        {
            var cache = new LoadingStepFactoryCache();
            cache.Add(new BaseFactory());
            var result = cache.Contains(new BaseFactory());

            Assert.IsTrue(result);
        }

        [Test]
        public void ContainsCachedFactoryReturnsFalseAfterClear()
        {
            var cache = new LoadingStepFactoryCache();
            cache.Add(new BaseFactory());
            cache.Clear();

            var result = cache.Contains(new BaseFactory());

            Assert.IsFalse(result);
        }

        [Test]
        public void ContainsFactoryWhenCachedDerivedFactoryReturnsFalse()
        {
            var cache = new LoadingStepFactoryCache();
            cache.Add(new DerivedFactory());
            cache.Clear();

            var result = cache.Contains(new BaseFactory());

            Assert.IsFalse(result);
        }

        [Test]
        public void ContainsFactoryWhenCachedBaseFactoryReturnsFalse()
        {
            var cache = new LoadingStepFactoryCache();
            cache.Add(new BaseFactory());
            cache.Clear();

            var result = cache.Contains(new DerivedFactory());

            Assert.IsFalse(result);
        }
        
        [Test]
        public void ContainsBaseFactoryWhenCachedDerivedFactoryWithBaseReturnsFalse()
        {
            var cache = new LoadingStepFactoryCache();
            cache.Add(new BaseFactory());
            BaseFactory baseFactory = new DerivedFactory();
            var result = cache.Contains(baseFactory);

            Assert.IsFalse(result);
        }
        #endregion
    }
}
