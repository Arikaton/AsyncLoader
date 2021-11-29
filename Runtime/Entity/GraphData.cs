using System.Collections.Generic;
using System;

namespace LoadingModule.Entity
{
    public sealed class GraphData
    {
        // TODO: подумать над тем, действительно ли нужна рутовая нода. По-сути, в основном она используется только в Editor, а там её можно дорисовать без особых проблем.
        public readonly GraphNode RootNode;
        public readonly IReadOnlyList<GraphNode> Nodes;
        public int NodesCount => Nodes.Count;

        public GraphData(GraphNode rootNode, IReadOnlyList<GraphNode> nodeList)
        {
            RootNode = rootNode ?? throw new NullReferenceException();
            Nodes = nodeList ?? throw new NullReferenceException();
        }
    }
}