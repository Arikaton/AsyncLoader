using UnityEngine;

namespace LoadingModule.Editor.NodeEditor
{
    internal sealed class ConnectionPoint
    {
        internal Rect Rect;
        
        private const int ArrowWidth = 8;
        private const int ArrowHeight = 8;

        internal ConnectionPoint(Node node)
        {
            Rect = new Rect(0, 0, 0, 0);
        }
        internal void DrawArrow(GUIStyle arrowStyle)
        {
            GUI.Box(new Rect(Rect.x - ArrowWidth / 2f, Rect.y - ArrowHeight / 2f, ArrowWidth, ArrowHeight), "", arrowStyle);
        }
        
    }
}