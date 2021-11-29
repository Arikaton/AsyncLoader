using NUnit.Framework;
using LoadingModule.Editor.Entity;
using System.Linq;
using LoadingModule.Entity;
using LoadingModule.Tests.Entity.Utils.Factories;

namespace LoadingModule.Tests.Editor.Entity.Utils
{
    internal class GraphUtilsEditorTest
    {

        [Test]
        public void FindCycleDependenciesWithNotCycleGraphReturnFalse()
        {
            var loadingSteps = new StepFactoryTest_Good().CreateLoadingSteps();
            var graphData = GraphUtils.BuildGraph(loadingSteps);

            var haveCycleDependency = GraphUtilsEditor.FindCycleDependencies(graphData.RootNode);
            Assert.False(haveCycleDependency);
        }

        [Test]
        public void FindCycleDependenciesWithCycleGraphReturnTrue()
        {
            var loadingSteps = new StepFactoryTest_Cycle().CreateLoadingSteps();
            var graphData = GraphUtils.BuildGraph(loadingSteps);

            var haveCycleDependency = GraphUtilsEditor.FindCycleDependencies(graphData.RootNode);
            Assert.True(haveCycleDependency);
        }

        [Test]
        public void DeepFirstSearchMakesNotAllNodesReadyWithGoodFactory()
        {
            var loadingSteps = new StepFactoryTest_Good().CreateLoadingSteps();
            var graphData = GraphUtils.BuildGraph(loadingSteps);

            GraphUtilsEditor.FindCycleDependencies(graphData.RootNode);
            var haveNotReadyNode = graphData.Nodes.Any(loadingGraphNode => loadingGraphNode.State != GraphNodeState.Ready);
            Assert.False(haveNotReadyNode);
        }

        [Test]
        public void DeepFirstSearchMakesNotAllNodesReadyWithCycleFactory()
        {
            var loadingSteps = new StepFactoryTest_Cycle().CreateLoadingSteps();
            var graphData = GraphUtils.BuildGraph(loadingSteps);

            GraphUtilsEditor.FindCycleDependencies(graphData.RootNode);
            var haveNotReadyNode = graphData.Nodes.Any(loadingGraphNode => loadingGraphNode.State != GraphNodeState.Ready);
            Assert.True(haveNotReadyNode);
        }

        [Test]
        public void DeepFirstSearcCheckAllNodes()
        {
            var loadingSteps = new StepFactoryTest_Good().CreateLoadingSteps();
            var graphData = GraphUtils.BuildGraph(loadingSteps);
            var counter = 0;

            GraphUtilsEditor.ResetNodesState(graphData.Nodes);
            GraphUtilsEditor.DeepFirstSearch(graphData.RootNode, node =>
            {
                counter++;
            });
            Assert.True(counter == graphData.Nodes.Count + 1);
        }
    }
}
