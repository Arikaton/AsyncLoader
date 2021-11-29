using NUnit.Framework;
using System;
using LoadingModule.Entity;
using System.Collections.Generic;
using Moq;
using LoadingModule.Contracts;
using Moq.Protected;
using Cysharp.Threading.Tasks;
using LoadingModule.Tests.Entity.Utils.Factories;
using System.Linq;

namespace LoadingModule.Tests.Editor.Entity.Graph
{
    internal sealed class GraphUtilsTest
    {
        [Test]
        public void BuildGraphStepsIsNullThrowsException()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                GraphUtils.BuildGraph(null);
            });
        }

        [Test]
        public void BuildGraphStepsIsEmptyThrowsException()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                GraphUtils.BuildGraph(new List<LoadingStep>());
            });
        }

        [Test]
        public void BuildGraphThrowsNoException()
        {
            var loadingSteps = new List<LoadingStep>();

            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));
            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(null);

            loadingSteps.Add(loadingStepMock.Object);

            Assert.DoesNotThrow(() =>
            {
                GraphUtils.BuildGraph(loadingSteps);
            });
        }

        [Test]
        public void BuildGraphReturnsNotNull()
        {
            var loadingSteps = new List<LoadingStep>();

            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));
            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(null);

            loadingSteps.Add(loadingStepMock.Object);

            var graphData = GraphUtils.BuildGraph(loadingSteps);

            Assert.NotNull(graphData.RootNode);
        }

        [Test]
        public void BuildGraphReturnsGraphDataWithoutClosedNodes()
        {
            var loadingSteps = new StepFactoryTest_Good().CreateLoadingSteps();
            var graphData = GraphUtils.BuildGraph(loadingSteps);

            var haveClosedNode = graphData.Nodes.Any(node => node.Step == null && node.NextNodes == null);
            Assert.False(haveClosedNode);
        }

        [Test]
        public void BuildGraphReturnsGraphDataWithoutStepInRootNode()
        {
            var loadingSteps = new List<LoadingStep>();
            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));
            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(null);
            loadingSteps.Add(loadingStepMock.Object);

            var graphData = GraphUtils.BuildGraph(loadingSteps);

            Assert.Null(graphData.RootNode.Step);
        }

        [Test]
        public void BuildGraphReturnsGraphDataWithStepsInAllNodesExceptFirst()
        {
            var loadingSteps = new List<LoadingStep>();
            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));
            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(null);
            var loadingStep = loadingStepMock.Object;
            loadingSteps.Add(loadingStep);

            var graphData = GraphUtils.BuildGraph(loadingSteps);

            foreach (var node in graphData.Nodes)
            {
                if(node != graphData.RootNode)
                    Assert.NotNull(node.Step);
            }
        }

        [Test]
        public void BuildGraphWithLoadingStepsWithoutDependencyReturns()
        {

        }


        [Test]
        public void BuildGraphReturnsGraphDataWithSameLoadingSteps()
        {
            var loadingSteps = new List<LoadingStep>();
            var loadingStepMock = new Mock<LoadingStep>(typeof(ILoadingArtifact));
            loadingStepMock.Protected().Setup<UniTask<ILoadingArtifact>>("Load").Returns(null);
            var loadingStep = loadingStepMock.Object;
            loadingSteps.Add(loadingStep);

            var graphData = GraphUtils.BuildGraph(loadingSteps);

            foreach(var node in graphData.Nodes)
            {
                Assert.IsTrue(loadingSteps.Contains(node.Step));
            }
        }
    }
}
