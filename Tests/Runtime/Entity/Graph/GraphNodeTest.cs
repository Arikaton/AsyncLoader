using NUnit.Framework;
using LoadingModule.Entity;
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.TestTools;
using System.Collections;
using UnityEngine;
using LoadingModule.Tests.Entity.Utils;

namespace LoadingModule.Tests.Entity.Graph
{
    public class GraphNodeTest
    {
        [UnityTest]
        public IEnumerator LoadThrowsNoException() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);
            var node = new GraphNode(loadingStepMock.Object);

            try
            {
                await node.Load();
            }
            catch (Exception exp)
            {
                throw new AssertionException($"{Constants.LoadingModuleTag} GraphNode.Load Throws an Exception {{{exp}}}!");
            }
        });

        [UnityTest]
        public IEnumerator LoadCheckCallBack() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);

            bool error = false;
            bool succes = false;

            var node = new GraphNode(loadingStepMock.Object);
            node.OnError += (x, e) => error = true;
            node.OnEndLoading += x => succes = x.Step.LoadingStatus == LoadingStatus.Loaded;
            await node.Load();

            Assert.IsTrue(succes);
            Assert.IsFalse(error);
        });

        [UnityTest]
        public IEnumerator LoadCheckStatusIsLoaded() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);

            var node = new GraphNode(loadingStepMock.Object);
            await node.Load();

            Assert.AreEqual(LoadingStatus.Loaded, node.Step.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator LoadExceptionStepThrowsNoException() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepErrorMock = LoadingStepModel.CreateLoadingStepExceptionMock();
            var node = new GraphNode(loadingStepErrorMock.Object);

            try
            {
                await node.Load();
            }
            catch (Exception exp)
            {
                throw new AssertionException($"{Constants.LoadingModuleTag} GraphNode.Load Throws an Exception {{{exp}}}!");
            }
        });

        [UnityTest]
        public IEnumerator LoadExceptionStepCheckCallBack() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepErrorMock = LoadingStepModel.CreateLoadingStepExceptionMock();
            bool error = false;
            bool succes = false;

            var node = new GraphNode(loadingStepErrorMock.Object);
            node.OnError += (x, e) => error = true;
            node.OnEndLoading += x => succes = x.Step.LoadingStatus == LoadingStatus.Loaded;
            await node.Load();

            Assert.IsTrue(error);
            Assert.IsFalse(succes);
        });

        [UnityTest]
        public IEnumerator LoadExceptionStepCheckStatusIsAborted() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepErrorMock = LoadingStepModel.CreateLoadingStepExceptionMock();

            var node = new GraphNode(loadingStepErrorMock.Object);
            await node.Load();

            Assert.AreEqual(LoadingStatus.Aborted, node.Step.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator LoadAbortCheckStatusIsAborted() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(200);
            var node = new GraphNode(loadingStepMock.Object);

            UniTask.Run(async () =>
            {
                await UniTask.Delay(100);
                Debug.Log("Aborted!");
                node.Abort();
            });

            await node.Load();

            Assert.AreEqual(LoadingStatus.Aborted, node.Step.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator ResolveStartsLoading() => UniTask.ToCoroutine(async () =>
        {
            var loadingStepMock = LoadingStepModel.CreateLoadingStepMock(5);

            var node1 = new GraphNode(loadingStepMock.Object);
            var node2 = new GraphNode(loadingStepMock.Object);

            var node1NextNodes = new List<GraphNode>();
            node1NextNodes.Add(node2);
            node1.NextNodes = node1NextNodes;

            await node1.Load();
            await UniTask.Delay(15);

            Assert.AreEqual(LoadingStatus.Loaded, node2.Step.LoadingStatus);
        });

        [UnityTest]
        public IEnumerator LoadWithNullStepThrowsException() => UniTask.ToCoroutine(async () =>
        {
            var node1 = new GraphNode(null);
            Exception actualException = null;

            try
            {
                await node1.Load();
            }
            catch (Exception exeption)
            {
                actualException = exeption;
            }

            Assert.AreEqual(typeof(NullReferenceException), actualException.GetType());
        });
    }
}
