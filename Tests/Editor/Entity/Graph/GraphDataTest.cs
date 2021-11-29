using NUnit.Framework;
using LoadingModule.Entity;
using System.Collections.Generic;
using System;

namespace LoadingModule.Tests.Editor.Entity.Graph
{
    internal sealed class GraphDataTest
    {

        [Test]
        public void ConstructorRootNodeIsNullThrowsException()
        {
            var graphNodeList = new List<GraphNode>();

            Assert.Throws<NullReferenceException>(() =>
            {
                new GraphData(null, graphNodeList);
            });
        }

        [Test]
        public void ConstructorNodeListIsNullThrowsException()
        {
            var graphNode = new GraphNode(null);

            Assert.Throws<NullReferenceException>(() =>
            {
                new GraphData(graphNode, null);
            });
        }

        [Test]
        public void ConstructorThrowsNoException()
        {
            var graphNode = new GraphNode(null);
            var graphNodeList = new List<GraphNode>();

            Assert.DoesNotThrow(() =>
            {
                new GraphData(graphNode, graphNodeList);
            });
        }

    }
}
