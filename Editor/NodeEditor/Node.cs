using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoadingModule.Editor.NodeEditor
{
    internal sealed class Node
    {
        private const int NodeTextureRightPadding = 4;

        internal readonly List<ConnectionPoint> InConnectionPoints = new List<ConnectionPoint>();
        internal readonly List<ConnectionPoint> OutConnectionPoints = new List<ConnectionPoint>();
        
        private Rect rect;
        private readonly string artifactName;
        private readonly string stepName;

        public Node(Rect rect, string artifactName, string stepName)
        {
            if (artifactName == null)
                throw new NullReferenceException();

            if (stepName == null)
                throw new NullReferenceException();

            this.rect = rect;
            this.artifactName = artifactName;
            this.stepName = stepName;
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        public void Draw(GUIStyle nodeStyle, string durationInMilliseconds = "")
        {
            GUI.Box(rect, "", nodeStyle);
            GUIStyle textStyle = GUI.skin.GetStyle("Label");
            textStyle.normal.textColor = Color.black;
            textStyle.alignment = TextAnchor.UpperCenter;
            textStyle.fontSize = 16;
            GUI.Label(new Rect(rect.x, rect.y + 5, rect.width, 23), stepName, textStyle);
            GUI.Label(new Rect(rect.x, rect.y + 28, rect.width, 23), artifactName, textStyle);
            GUI.Label(new Rect(rect.x, rect.y + 51, rect.width, 23), durationInMilliseconds, textStyle);
        }

        public void DefineConnectionsPosition()
        {
            for (var i = 0; i < InConnectionPoints.Count; i++)
            {
                InConnectionPoints[i].Rect.y = rect.y + (i + 1) * rect.height / (InConnectionPoints.Count + 1);
                InConnectionPoints[i].Rect.x = rect.x - NodeTextureRightPadding;
            }

            for (var i = 0; i < OutConnectionPoints.Count; i++)
            {
                OutConnectionPoints[i].Rect.y = rect.y + rect.height / 2f;   
                OutConnectionPoints[i].Rect.x = rect.x + rect.width - NodeTextureRightPadding;
            }
        }
    }
}