using NUnit.Framework;
using LoadingModule.Editor.NodeEditor;
using UnityEngine;
using System;

namespace LoadingModule.Tests.Editor.NodeEditor
{
    internal sealed class NodeTest
    {
        [Test]
        public void ConstructorArtifactNameIsNullThrowsException()
        {
            Rect rect = new Rect();
            string artifactName = null;
            string stepName = "stepName";

            Assert.Throws<NullReferenceException>(() =>
            {
                new Node(rect, artifactName, stepName);
            });
        }

        [Test]
        public void ConstructorStepNameIsNullThrowsException()
        {
            Rect rect = new Rect();
            string artifactName = "artifactName";
            string stepName = null;

            Assert.Throws<NullReferenceException>(() =>
            {
                new Node(rect, artifactName, stepName);
            });
        }

        [Test]
        public void ConstructorThrowsNoException()
        {
            Rect rect = new Rect();
            string artifactName = "artifactName";
            string stepName = "stepName";

            Assert.DoesNotThrow(() =>
            {
                new Node(rect, artifactName, stepName);
            });
        }
    }
}
