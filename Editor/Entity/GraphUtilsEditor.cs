using System;
using System.Collections.Generic;
using LoadingModule.Entity;
using UnityEngine;

namespace LoadingModule.Editor.Entity
{
    internal static class GraphUtilsEditor
    {
        internal static bool FindCycleDependencies(GraphNode node)
        {
            if (node is null)
                return false;
            
            node.State = GraphNodeState.Prepared;

            var nodeWasCycle = false;

            if (node.NextNodes != null)
            {
                foreach (var nextNode in node.NextNodes)
                {
                    if (nextNode.State == GraphNodeState.Prepared)
                    {
                        Debug.LogWarning($"{Constants.LoadingModuleTag} A cyclic dependence is found closed on " + nextNode.Step.GetType().Name);
                        nextNode.State = GraphNodeState.Cycle;
                    }

                    var nextNodeWasCycle = nextNode.State == GraphNodeState.Cycle;

                    if (nextNode.State == GraphNodeState.NotChecked)
                    {
                        bool nextNodeBecameCycle = FindCycleDependencies(nextNode);
                        if (nextNodeBecameCycle)
                        {
                            if (node.State == GraphNodeState.Cycle)
                                nodeWasCycle = true;
                            node.State = GraphNodeState.Cycle;
                        }
                        else
                        {
                            node.State = GraphNodeState.Ready;
                        }
                    }

                    if (nextNodeWasCycle)
                    {
                        if (node.State == GraphNodeState.Cycle)
                            nodeWasCycle = true;
                        node.State = GraphNodeState.Cycle;
                    }
                }
            }
            else
            {
                node.State = GraphNodeState.Ready;
            }

            if (nodeWasCycle)
            {
                node.State = GraphNodeState.Cycle;
                return true;
            }

            if (node.State != GraphNodeState.Cycle)
                node.State = GraphNodeState.Ready;
            return node.State == GraphNodeState.Cycle;
        }
        internal static void DeepFirstSearch(GraphNode rootNode, Action<GraphNode> onEnterNode = null, Action<GraphNode> onExitNode = null)
        {
            if (rootNode == null)
                return;
            onEnterNode?.Invoke(rootNode);
            rootNode.State = GraphNodeState.Prepared;
            if (rootNode.NextNodes != null)
            {
                foreach (var nextNode in rootNode.NextNodes)
                {
                    if (nextNode.State == GraphNodeState.NotChecked)
                        DeepFirstSearch(nextNode, onEnterNode, onExitNode);
                }
            }

            rootNode.State = GraphNodeState.Ready;
            onExitNode?.Invoke(rootNode);
        }
        internal static void ResetNodesState(IReadOnlyList<GraphNode> nodes)
        {
            if (nodes is null)
                return;
            foreach (var node in nodes)
            {
                node.State = GraphNodeState.NotChecked;
            }
        }
    }
}