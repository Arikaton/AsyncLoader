using NUnit.Framework;
using System;
using LoadingModule.Contracts;
using LoadingModule.Entity;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using System.Threading;
using Moq;
using Moq.Protected;

namespace LoadingModule.Tests.Entity.Utils
{
    public sealed class LoadingStepTest
    {
        [UnityTest]
        public IEnumerator LoadReturnNullThrowsException() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));

            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(async () =>
            {
                await UniTask.Delay(5);
                return null;
            });

            var loadingStep = loadingStepMock.Object;
            Exception actualException = null;

            try
            {
                await loadingStep.LoadInternal();
            }
            catch (NullReferenceException exception)
            {
                actualException = exception;
            }

            Assert.NotNull(actualException);
        });

        [UnityTest]
        public IEnumerator LoadInternalExceptStatusLoaded() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);
            var loadingStep = loadingStepMock.Object;

            await loadingStep.LoadInternal();

            Assert.AreEqual(LoadingStatus.Loaded, loadingStep.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator LoadInternalReturnsArtifact() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);
            var loadingStep = loadingStepMock.Object;

            var result = await loadingStep.LoadInternal();

            Assert.IsTrue(result is ILoadingArtifact);
        });

        [UnityTest]
        public IEnumerator LoadInternalWithAbortReturnsNull() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(200);
            var loadingStep = loadingStepMock.Object;

            UniTask.Run(async () =>
            {
                await UniTask.Delay(100);
                loadingStep.Abort();
                Debug.Log("Aborted!");
            });

            var result = await loadingStep.LoadInternal();

            Assert.AreEqual(null, result);
        });

        [UnityTest]
        public IEnumerator LoadInternalWithPreAbortRenurnsNull() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);
            var loadingStep = loadingStepMock.Object;

            loadingStep.Abort();
            var result = await loadingStep.LoadInternal();

            Assert.AreEqual(null, result);
        });

        [UnityTest]
        public IEnumerator LoadInternalWithAbortExceptStatusAborted() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(200);
            var loadingStep = loadingStepMock.Object;

            UniTask.Run(async () =>
            {
                await UniTask.Delay(100);
                loadingStep.Abort();
                Debug.Log("Aborted!");
            });

            await loadingStep.LoadInternal();

            Assert.AreEqual(LoadingStatus.Aborted, loadingStep.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator LoadInternalWithPreAbortExceptStatusAborted() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);
            var loadingStep = loadingStepMock.Object;

            loadingStep.Abort();
            await loadingStep.LoadInternal();

            Assert.AreEqual(LoadingStatus.Aborted, loadingStep.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator LoadInternalWithExceptionExceptThrowsException() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepExceptionMock();
            var loadingStep = loadingStepMock.Object;
            Exception actualException = null;

            try
            {
                await loadingStep.LoadInternal();
            }
            catch (Exception exception)
            {
                actualException = exception;
            }

            Assert.NotNull(actualException);
        });

        [UnityTest]
        public IEnumerator LoadInternalWithExceptionExceptStatusAborted() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepExceptionMock();
            var loadingStep = loadingStepMock.Object;

            try
            {
                await loadingStep.LoadInternal();
            }
            catch (Exception exception)
            {

            }

            Assert.AreEqual(LoadingStatus.Aborted, loadingStep.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator LoadInternalSecondTimeThrowsException() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(200);
            var loadingStep = loadingStepMock.Object;
            Exception actualException = null;

            UniTask.Run(async () =>
            {
                await loadingStep.LoadInternal();
            });

            await UniTask.Delay(100);

            try
            {
                await loadingStep.LoadInternal();
            }
            catch(ThreadInterruptedException exception)
            {
                actualException = exception;
            }

            Assert.NotNull(actualException);
        });


        
    }
}
