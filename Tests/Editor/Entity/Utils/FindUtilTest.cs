using NUnit.Framework;
using LoadingModule.Editor.Entity.Utils;

namespace LoadingModule.Tests.Editor.Entity.Utils
{
    internal class FindUtilTest
    {
        [Test]
        public void GetAllFactoryInstancesThrowsNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var factories = FindUtils.GetAllFactoryInstances();
            });
        }

        [Test]
        public void GetAllFactoryInstancesReturnNotNullOrEmptyList()
        {
            var factories = FindUtils.GetAllFactoryInstances();
            Assert.NotNull(factories);
            Assert.NotZero(factories.Count);
        }

        [Test]
        public void GetAllFactoryNamesThrowsNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var factoryNames = FindUtils.GetAllFactoryNames();
            });
        }

        [Test]
        public void GetAllFactoryNamesReturnNotNullOrEmptyList()
        {
            var factoryNames = FindUtils.GetAllFactoryNames();
            Assert.NotNull(factoryNames);
            Assert.NotZero(factoryNames.Count);
        }

        [Test]
        public void GetVisibleFactoryInstancesThrowsNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var factories = FindUtils.GetVisibleFactoryInstances();
            });
        }

        //[Test]
        //public void GetVisibleFactoryInstancesReturnNotNullOrEmptyList()
        //{
        //    var factories = FindUtils.GetVisibleFactoryInstances();
        //    Assert.NotNull(factories);
        //    Assert.NotZero(factories.Count);
        //}

        [Test]
        public void GetVisibleFactoryNamesThrowsNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var factoryNames = FindUtils.GetVisibleFactoryNames();
            });
        }

        //[Test]
        //public void GetVisibleFactoryNamesReturnNotNullOrEmptyList()
        //{
        //    var factoryNames = FindUtils.GetVisibleFactoryNames();
        //    Assert.NotNull(factoryNames);
        //    Assert.NotZero(factoryNames.Count);
        //}

        [Test]
        public void GetStepNamesThrowsNoException()
        {
            Assert.DoesNotThrow(() =>
            {
                var stepNames = FindUtils.GetStepNames();
            });
        }

        [Test]
        public void GetStepNamesReturnNotNullOrEmptyList()
        {
            var stepNames = FindUtils.GetStepNames();
            Assert.NotNull(stepNames);
            Assert.NotZero(stepNames.Count);
        }
    }
}
