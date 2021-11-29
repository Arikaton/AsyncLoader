using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LoadingModule.Entity
{
    internal static class GraphUtils
    {
        internal static GraphData BuildGraph(List<LoadingStep> steps)
        {
            if (StepsNullOrEmpty(steps))
            {
                throw new DataException($"{Constants.LoadingModuleTag} Steps null or empty");
            }
            
            var allNodes = new List<GraphNode>();
            var stepDict = new Dictionary<Type, List<GraphNode>>();
            var firstNodes = new List<GraphNode>();
            var startNode = new GraphNode(null);
            
            foreach (var loadingStep in steps)
            {
                var node = new GraphNode(loadingStep);
                allNodes.Add(node);
                
                if (!loadingStep.HasDependencies)
                    firstNodes.Add(node);
                
                foreach (var stepDependency in node.Step.Dependencies)
                {
                    if (!stepDict.ContainsKey(stepDependency.ArtifactType))
                        stepDict[stepDependency.ArtifactType] = new List<GraphNode>();
                    stepDict[stepDependency.ArtifactType].Add(node);
                }
            }

            AddDependenciesToNodes(firstNodes, stepDict);
            
            startNode.NextNodes = firstNodes;
            
            return new GraphData(startNode, allNodes);
        }

        private static void AddDependenciesToNodes(List<GraphNode> firstNodes, Dictionary<Type, List<GraphNode>> stepDict)
        {
            foreach (var node in firstNodes)
            {
                if (stepDict.ContainsKey(node.Step.ArtifactType))
                    node.NextNodes = stepDict[node.Step.ArtifactType];
            }
            
            foreach (var nodes in stepDict.Values.ToList())
            {
                foreach (var node in nodes)
                {
                    if (stepDict.ContainsKey(node.Step.ArtifactType))
                        node.NextNodes = stepDict[node.Step.ArtifactType];
                }
            }
        }
        
        private static bool StepsNullOrEmpty(List<LoadingStep> steps)
        {
            return steps == null || steps.Count == 0;
        }
    }
}