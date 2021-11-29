using UnityEditor;
using UnityEngine;

namespace LoadingModule.Editor.NodeEditor
{
    internal sealed class Connection
    {
        private readonly ConnectionPoint inPoint;
        private readonly ConnectionPoint outPoint;
        private readonly Color color;

        internal Connection(ConnectionPoint inPoint, ConnectionPoint outPoint)
        {
            this.inPoint = inPoint;
            this.outPoint = outPoint;
            color = Color.HSVToRGB(Random.Range(0, 1f), 30f / 100, 60f / 100);
        }
        internal void DrawBezierLine()
        {
            var startPosition = inPoint.Rect.center;
            var endPosition = outPoint.Rect.center;
            var startTangent = inPoint.Rect.center + Vector2.left * 50f;
            var endTangent = outPoint.Rect.center - Vector2.left * 50f;
            if (startPosition.x < endPosition.x)
            {
                startTangent += new Vector2(-100, 100);
                endTangent += new Vector2(100, 100); 
            }
            Handles.DrawBezier(
                startPosition,
                endPosition,
                startTangent,
                endTangent,
                color,
                null,
                2f
            );
        }
        internal void DrawArrow(GUIStyle arrowStyle)
        {
            inPoint.DrawArrow(arrowStyle);
        }
    }
}