using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using LoadingModule.Contracts;
using LoadingModule.Editor.Entity;
using LoadingModule.Entity;

namespace LoadingModule.Editor.NodeEditor
{
    internal sealed class NodeBasedEditor : EditorWindow
    {
        private const float DragSensitivity = 0.5f;
        private const int NodeWidth = 200;
        private const int NodeHeight = 80;
        private const int NodeSpaceX = 50;
        private const int NodeSpaceY = 40;

        private Dictionary<Node, GraphNode> graphNodeDataByVisualNode = new Dictionary<Node, GraphNode>();
        private List<Node> nodes;
        private List<Connection> connections;
        private Vector2 offset;
        private Vector2 drag;
        private AbstractLoadingStepFactory factory;
        
        private GUIStyle arrowStyle;
        private GUIStyle nodeStyle;
        private GUIStyle nodeCycleStyle;
        private GUIStyle nodeLoadedStyle;

        internal void SetFactory(AbstractLoadingStepFactory loadingStepFactory)
        {
            factory = loadingStepFactory;
            nodes = new List<Node>();
            connections = new List<Connection>();
            var graphData = GraphUtils.BuildGraph(factory.CreateLoadingSteps());
            PrepareVisual(graphData);
            OnDrag(new Vector2(15, 300));
        }
        internal void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.gray);
            
            DrawConnections(arrowStyle);
            DrawNodes();
            DrawTotalLoadingTimeLabel();

            ProcessEvents(Event.current);
            if (GUI.changed) Repaint();
        }

        #region private methods
        private void OnEnable()
        {
            DefineStyles();
            LoadingInitializer.OnInitFinished += OnGameLoaded;
        }
        private void OnDisable()
        {
            LoadingInitializer.OnInitFinished -= OnGameLoaded;
            PerformanceMeter.Instance.UnsubscribeFromLoadingController();
        }
        private void DrawTotalLoadingTimeLabel()
        {
            float totalLoadingTime = PerformanceMeter.Instance.TotalLoadingTime;
            if (totalLoadingTime != 0)
            {
                GUI.Label(new Rect(0, 0, 300, 25), $"Total loading time: {totalLoadingTime.ToString()}ms ");
            }
        }
        private void OnGameLoaded(LoadingController loadingController)
        {
            PrepareVisual(loadingController.GraphData);
            PerformanceMeter.Instance.UnsubscribeFromLoadingController();
            PerformanceMeter.Instance.ClearData();
            PerformanceMeter.Instance.SubscribeOnLoadingController(loadingController);
        }
        private void DefineStyles()
        {
            arrowStyle = new GUIStyle();
            var arrowTexture = Resources.Load<Texture2D>("LoadingModuleAssets/Arrow");
            arrowStyle.normal.background = arrowTexture;

            nodeStyle = new GUIStyle();
            var nodeTexture = Resources.Load<Texture2D>("LoadingModuleAssets/NodeBG");
            nodeStyle.normal.background = nodeTexture;
            
            nodeCycleStyle = new GUIStyle();
            var nodeCycleTexture = Resources.Load<Texture2D>("LoadingModuleAssets/NodeCycleBG");
            nodeCycleStyle.normal.background = nodeCycleTexture;
            
            nodeLoadedStyle = new GUIStyle();
            var nodeLoadedTexture = Resources.Load<Texture2D>("LoadingModuleAssets/NodeLoadedBG");
            nodeLoadedStyle.normal.background = nodeLoadedTexture;
        }
        private List<List<GraphNode>> SortNodeByDepth(GraphData graphData)
        {
            var nodeListSortedByDepth = new List<List<GraphNode>>();
            var depth = 0;
            GraphUtilsEditor.ResetNodesState(graphData.Nodes);
            GraphUtilsEditor.DeepFirstSearch(
                graphData.RootNode,
                node =>
                {
                    if (depth + 1 > nodeListSortedByDepth.Count)
                        nodeListSortedByDepth.Add(new List<GraphNode>());
                    nodeListSortedByDepth[depth].Add(node);
                    depth++;
                },
                node => depth--
            );
            return nodeListSortedByDepth;
        }
        private void PrepareVisual(GraphData graphData)
        {
            var nodeListSortedByDepth = SortNodeByDepth(graphData);
            var graphNodeToVisual = GenerateVisualNodes(nodeListSortedByDepth, graphData.RootNode);
            graphNodeDataByVisualNode = graphNodeToVisual.ToDictionary(x => x.Value, x => x.Key);
            GenerateConnections(graphNodeToVisual);
            GraphUtilsEditor.ResetNodesState(graphData.Nodes);
            GraphUtilsEditor.FindCycleDependencies(graphData.RootNode);
        }
        private void GenerateConnections(Dictionary<GraphNode, Node> graphNodeToVisual)
        {
            connections = new List<Connection>();
            foreach (var node in graphNodeToVisual.Keys)
            {
                if (node.NextNodes != null)
                {
                    foreach (var nextNode in node.NextNodes)
                    {
                        var outConnection = new ConnectionPoint(graphNodeToVisual[node]);
                        var inConnection = new ConnectionPoint(graphNodeToVisual[nextNode]);
                        var connection = new Connection(inConnection, outConnection);
                        connections.Add(connection);
                        graphNodeToVisual[nextNode].InConnectionPoints.Add(inConnection);
                        graphNodeToVisual[node].OutConnectionPoints.Add(outConnection);
                    }
                }
            }
        }
        private Dictionary<GraphNode, Node> GenerateVisualNodes(List<List<GraphNode>> nodeListSortedByDepth, GraphNode firstNode)
        {
            nodes = new List<Node>();
            var graphNodeToVisual = new Dictionary<GraphNode, Node>();
            for (var i = 0; i < nodeListSortedByDepth.Count; i++)
            {
                var widthCount = nodeListSortedByDepth[i].Count;
                float totalHeight = widthCount * NodeHeight + (widthCount - 1) * NodeSpaceY;
                var startPos = (0 - totalHeight) / 2 + NodeSpaceY;

                for (var j = 0; j < nodeListSortedByDepth[i].Count; j++)
                {
                    var node = nodeListSortedByDepth[i][j];
                    var nodePos = new Vector2(i * (NodeSpaceX + NodeWidth), startPos + j * (NodeSpaceY + NodeHeight));
                    var artifactName = node == firstNode ? "Root node" : node.Step.ArtifactType.Name.Split('.').Last();
                    var stepName = node == firstNode ? "" : node.Step.GetType().Name;
                    var newVisualNode = new Node(new Rect(nodePos, new Vector2(NodeWidth, NodeHeight)), artifactName, stepName);
                    graphNodeToVisual.Add(node, newVisualNode);
                    nodes.Add(newVisualNode);
                }
            }

            return graphNodeToVisual;
        }
        private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(2000 / gridSpacing);
            int heightDivs = Mathf.CeilToInt(2000 / gridSpacing);
            
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            offset += drag * 0.5f;
            Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, 2000, 0f) + newOffset);
            }

            for (int j = 0; j < heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(2000, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }
        private void DrawNodes()
        {
            if (nodes == null) return;
            
            for (var i = 0; i < nodes.Count; i++)
            {
                var graphNode = graphNodeDataByVisualNode[nodes[i]];
                if (graphNode.State == GraphNodeState.Cycle)
                {
                    nodes[i].Draw(nodeCycleStyle);
                }
                else
                {
                    if (graphNode.Step != null && graphNode.Step.LoadingStatus == LoadingStatus.Loaded)
                    {
                        string nodeloadingTime = "";
                        if (PerformanceMeter.Instance.GraphNodeLoadingDurationInfo.ContainsKey(graphNode))
                        {
                            nodeloadingTime = PerformanceMeter.Instance.GraphNodeLoadingDurationInfo[graphNode].ToString();
                        }
                        else
                        {
                            nodeloadingTime = "0";
                        }
                        var performance = $"{nodeloadingTime}ms";
                        nodes[i].Draw(nodeLoadedStyle, performance);
                    }
                    else
                    {
                        nodes[i].Draw(nodeStyle);
                    }
                }
                nodes[i].DefineConnectionsPosition();
            }
        }
        private void DrawConnections(GUIStyle arrowStyle)
        {
            if (connections != null)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].DrawBezierLine();
                }
                for (int i = 0; i < connections.Count; i++)
                {
                    connections[i].DrawArrow(arrowStyle);
                } 
            }
        }
        private void ProcessEvents(Event e)
        {
            drag = Vector2.zero;

            if (e.button == 0)
                OnDrag(e.delta * DragSensitivity);
        }
        private void OnDrag(Vector2 delta)
        {
            drag = delta;

            if (nodes != null)
            {
                for (int i = 0; i < nodes.Count; i++)
                {
                    nodes[i].Drag(delta);
                }
            }

            GUI.changed = true;
        }
        #endregion
    }
}