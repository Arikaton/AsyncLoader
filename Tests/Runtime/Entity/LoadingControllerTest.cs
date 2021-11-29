using Cysharp.Threading.Tasks;
using NUnit.Framework;
using LoadingModule.Contracts;
using LoadingModule.Entity;
using LoadingModule.Tests.Entity.Utils.Factories;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace LoadingModule.Tests.Entity
{
    public sealed class LoadingControllerTest
    {
        [UnityTest]
        public IEnumerator LoadExceptionFactoryCheckResult() => UniTask.ToCoroutine(async () =>
       {
           LoadingInitializer.ClearFactoryCache();
           var controller = LoadingInitializer.Init(new StepFactoryTest_ThrowException());
           bool end = false;

           controller.EventSystem.OnEndLoading += x => end = true;

           controller.Load();

           while (!end)
           {
               await UniTask.Delay(5);
               VerifyLoading(controller.GraphData);
           }

           VerifyCompleteLoad(controller.GraphData);
        });

        [UnityTest]
        public IEnumerator LoadExceptionFactoryCheckCallBack() => UniTask.ToCoroutine(async () =>
        {
            LoadingInitializer.ClearFactoryCache();
            var controller = LoadingInitializer.Init(new StepFactoryTest_ThrowException());
            bool end = false;
            bool succes = false;
            bool error = false;

            controller.EventSystem.OnEndLoading += x =>
            {
                end = true;
                succes = x;
            };
            controller.EventSystem.OnError += x => error = true;
            controller.Load();

            while (!end)
            {
                await UniTask.Delay(5);
            }

            Assert.IsTrue(error);
            Assert.IsFalse(succes);
        });

        [UnityTest]
        public IEnumerator LoadWithAbortCheckResult() => UniTask.ToCoroutine(async () =>
        {
            LoadingInitializer.ClearFactoryCache();
            var controller = LoadingInitializer.Init(new StepFactoryTest_Good(100));
            bool end = false;

            controller.EventSystem.OnEndLoading += x => end = true;
            controller.Load();

            await UniTask.Delay(250);

            controller.Abort();

            while (!end)
            {
                await UniTask.Delay(5);
                VerifyLoading(controller.GraphData);
            }

            VerifyCompleteLoad(controller.GraphData);
        });

        [UnityTest]
        public IEnumerator LoadWithAbortCheckCallBack() => UniTask.ToCoroutine(async () =>
        {
            LoadingInitializer.ClearFactoryCache();
            var controller = LoadingInitializer.Init(new StepFactoryTest_Good(100));
            bool end = false;
            bool succes = false;
            bool error = false;

            controller.EventSystem.OnEndLoading += x =>
            {
                end = true;
                succes = x;
            };

            controller.EventSystem.OnError += x => error = true;
            controller.Load();

            await UniTask.Delay(250);
            controller.Abort();

            while (!end)
            {
                await UniTask.Delay(5);
            }

            Assert.IsFalse(error);
            Assert.IsFalse(succes);
        });

        [UnityTest]
        public IEnumerator LoadCheckCallBack() => UniTask.ToCoroutine(async () =>
        {
            LoadingInitializer.ClearFactoryCache();
            var controller = LoadingInitializer.Init(new StepFactoryTest_Good());
            bool end = false;
            bool succes = false;
            bool error = false;

            controller.EventSystem.OnEndLoading += x =>
            {
                end = true;
                succes = x;
            };
            controller.EventSystem.OnError += x => error = true;
            controller.Load();

            while (!end)
            {
                await UniTask.Delay(5);
            }

            Assert.IsFalse(error);
            Assert.IsTrue(succes);
        });


        [UnityTest]
        public IEnumerator LoadCheckResult() => UniTask.ToCoroutine(async () =>
        {
            LoadingInitializer.ClearFactoryCache();
            var controller = LoadingInitializer.Init(new StepFactoryTest_Good());
            bool end = false;

            controller.EventSystem.OnEndLoading += x => end = true;

            controller.Load();

            while (!end)
            {
                await UniTask.Delay(5);
                VerifyLoading(controller.GraphData);
            }

            VerifyCompleteLoad(controller.GraphData);
        });


        private void VerifyCompleteLoad(GraphData graphData)
        {
            VerifyCompleteLoad(null, graphData.RootNode.NextNodes);
        }

        private void VerifyLoading(GraphData graphData)
        {
            VerifyLoading(null, null, graphData.RootNode.NextNodes);
        }

        private void VerifyCompleteLoad(IEnumerable<GraphNode> abortedNodes, IEnumerable<GraphNode> loadedNodes)
        {
            var abortedChilds = new List<GraphNode>();
            var loadedChilds = new List<GraphNode>();

            if (abortedNodes != null && abortedNodes.Count() != 0)
            {
                foreach (var node in abortedNodes)
                {
                    if (node.NextNodes == null || node.NextNodes.Count == 0) continue;

                    foreach (var child in node.NextNodes)
                    {
                        if (!abortedChilds.Contains(child))
                        {
                            Assert.AreEqual(LoadingStatus.Aborted, child.Step.LoadingStatus);
                            abortedChilds.Add(child);
                        }
                    }
                }
            }

            if (loadedNodes != null && loadedNodes.Count() != 0)
            {
                foreach (var node in loadedNodes)
                {
                    if (node.NextNodes == null || node.NextNodes.Count == 0) continue;

                    foreach (var child in node.NextNodes)
                    {
                        if (!(abortedChilds.Contains(child) || loadedChilds.Contains(child)))
                        {
                            switch (child.Step.LoadingStatus)
                            {
                                case LoadingStatus.Loaded:
                                    {
                                        loadedChilds.Add(child);
                                        break;
                                    }
                                case LoadingStatus.Aborted:
                                    {
                                        abortedChilds.Add(child);
                                        break;
                                    }
                                case LoadingStatus.Loading:
                                    {
                                        throw new AssertionException($"{Constants.LoadingModuleTag} LoadingStep <{child.Step}> can't have status \'Loading\' after complition of load");
                                    }
                                case LoadingStatus.NotLoaded:
                                    {
                                        throw new AssertionException($"{Constants.LoadingModuleTag} LoadingStep <{child.Step}> can't have status \'NotLoaded\' after complition of load");
                                    }
                                default:
                                    throw new InvalidEnumArgumentException();
                            }
                        }
                    }
                }
            }

            abortedChilds = abortedChilds.Count > 0 ? abortedChilds : null;
            loadedChilds = loadedChilds.Count > 0 ? loadedChilds : null;

            if (abortedChilds != null || loadedChilds != null)
            {
                VerifyCompleteLoad(abortedNodes, loadedChilds);
            }
        }

        private void VerifyLoading(IEnumerable<GraphNode> abortedNodes, IEnumerable<GraphNode> notLoadedNodes, IEnumerable<GraphNode> loadedNodes)
        {
            var abortedChilds = new List<GraphNode>();
            var notLoadedChilds = new List<GraphNode>();
            var loadedChilds = new List<GraphNode>();

            if (abortedNodes != null && abortedNodes.Count() != 0)
            {
                foreach (var node in abortedNodes)
                {
                    if (node.NextNodes == null || node.NextNodes.Count == 0) continue;

                    foreach (var child in node.NextNodes)
                    {
                        if (!abortedChilds.Contains(child))
                        {
                            Assert.AreEqual(LoadingStatus.Aborted, child.Step.LoadingStatus);
                            abortedChilds.Add(child);
                        }
                    }
                }
            }

            if (notLoadedNodes != null && notLoadedNodes.Count() != 0)
            {
                foreach (var node in notLoadedNodes)
                {
                    if (node.NextNodes == null || node.NextNodes.Count == 0) continue;

                    foreach (var child in node.NextNodes)
                    {
                        if (!(abortedChilds.Contains(child) || notLoadedChilds.Contains(child)))
                        {
                            Assert.AreEqual(LoadingStatus.NotLoaded, child.Step.LoadingStatus);
                            notLoadedChilds.Add(child);
                        }
                    }
                }
            }

            if (loadedNodes != null && loadedNodes.Count() != 0)
            {
                foreach (var node in loadedNodes)
                {
                    if (node.NextNodes == null || node.NextNodes.Count == 0) continue;

                    foreach (var child in node.NextNodes)
                    {
                        if (!(abortedChilds.Contains(child) || notLoadedChilds.Contains(child) || loadedChilds.Contains(child)))
                        {
                            switch (child.Step.LoadingStatus)
                            {
                                case LoadingStatus.Loaded:
                                    {
                                        loadedChilds.Add(child);
                                        break;
                                    }
                                case LoadingStatus.Aborted:
                                    {
                                        abortedChilds.Add(child);
                                        break;
                                    }
                                case LoadingStatus.Loading:
                                case LoadingStatus.NotLoaded:
                                    {
                                        notLoadedChilds.Add(child);
                                        break;
                                    }
                                default:
                                    throw new InvalidEnumArgumentException();
                            }
                        }
                    }
                }
            }

            abortedChilds = abortedChilds.Count > 0 ? abortedChilds : null;
            notLoadedChilds = notLoadedChilds.Count > 0 ? notLoadedChilds : null;
            loadedChilds = loadedChilds.Count > 0 ? loadedChilds : null;

            if (abortedChilds != null || notLoadedChilds != null || loadedChilds != null)
            {
                VerifyLoading(abortedNodes, notLoadedChilds, loadedChilds);
            }
        }
    }
}
