using NUnit.Framework;
using LoadingModule.Entity;
using System;
using System.Collections.Generic;

namespace LoadingModule.Tests.Editor.Entity
{
    internal sealed class LoadingControllerTest
    {
        [Test]
        public void ConstructorGraphDataIsNullThrowsException()
        {
            GraphData graphData = null;
            EventSystem eventSystem = new EventSystem();

            Assert.Throws<NullReferenceException>(() =>
            {
                new LoadingController(graphData, eventSystem);
            });
        }

        [Test]
        public void ConstructorEventSystemIsNullThrowsException()
        {
            EventSystem eventSystem = null;
            GraphData graphData = new GraphData(new GraphNode(null), 
                new List<GraphNode>());

            Assert.Throws<NullReferenceException>(() =>
            {
                new LoadingController(graphData, eventSystem);
            });
        }

        [Test]
        public void ConstructorThrowsNoException()
        {
            EventSystem eventSystem = new EventSystem();
            GraphData graphData = new GraphData(new GraphNode(null),
                new List<GraphNode>());

            Assert.DoesNotThrow(() =>
            {
                new LoadingController(graphData, eventSystem);
            });
        }
    }
}
