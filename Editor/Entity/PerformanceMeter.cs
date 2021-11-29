using System.Collections.Generic;
using System.Diagnostics;
using LoadingModule.Entity;
using LoadingModule.Editor.Entity.Utils;

namespace LoadingModule.Editor.Entity
{
    internal sealed class PerformanceMeter
    {
        internal static PerformanceMeter Instance => instance ?? new PerformanceMeter();

        private static PerformanceMeter instance;

        internal Dictionary<GraphNode, float> GraphNodeLoadingDurationInfo { get; private set; }
        internal float TotalLoadingTime { get; private set; }
        private readonly Dictionary<GraphNode, Stopwatch> performanceDatas = new Dictionary<GraphNode, Stopwatch>();
        private Stopwatch commonStopWatch;
        private LoadingController loadingController;

        private PerformanceMeter()
        {
            instance = this;
            GraphNodeLoadingDurationInfo = new Dictionary<GraphNode, float>();
        }
        internal void SubscribeOnLoadingController(LoadingController loadingController)
        {
            this.loadingController = loadingController;
            loadingController.EventSystem.EditorOnStartLoading += StartMeasure;
            loadingController.EventSystem.OnEndLoading += StopMeasure;
            loadingController.EventSystem.OnEndLoading += CreateLog;
            loadingController.OnAborted += ClearData;
            foreach (var graphNode in loadingController.GraphData.Nodes)
            {
                graphNode.OnStartLoading += StartMeasure;
                graphNode.OnEndLoading += StopMeasure;
            }
        }

        internal void UnsubscribeFromLoadingController()
        {
            if (loadingController == null)
                return;
            loadingController.EventSystem.EditorOnStartLoading -= StartMeasure;
            loadingController.EventSystem.OnEndLoading -= StopMeasure;
            loadingController.EventSystem.OnEndLoading -= CreateLog;
            loadingController.OnAborted -= ClearData;
            foreach (var graphNode in loadingController.GraphData.Nodes)
            {
                graphNode.OnStartLoading -= StartMeasure;
                graphNode.OnEndLoading -= StopMeasure;
            }
        }

        private void CreateLog(EndLoadHandler endLoadHandler)
        {
            LogUtils.CreateLogFromPerformanceMeter(this);
        }

        private void StartMeasure()
        {
            commonStopWatch = new Stopwatch();
            commonStopWatch.Start();
        }

        private void StopMeasure(EndLoadHandler endLoadHandler)
        {
            commonStopWatch.Stop();
            TotalLoadingTime = commonStopWatch.ElapsedMilliseconds;
        }

        private void StartMeasure(GraphNode graphNode)
        {
            Stopwatch stopwatch = new Stopwatch();
            performanceDatas.Add(graphNode, stopwatch);
            stopwatch.Start();
        }

        private void StopMeasure(GraphNode graphNode)
        {
            var stopwatch = performanceDatas[graphNode];
            stopwatch.Stop();
            if (GraphNodeLoadingDurationInfo.ContainsKey(graphNode))
                GraphNodeLoadingDurationInfo[graphNode] = stopwatch.ElapsedMilliseconds;
            else
                GraphNodeLoadingDurationInfo.Add(graphNode, stopwatch.ElapsedMilliseconds);
        }

        internal void ClearData()
        {
            foreach (var graphNode in performanceDatas.Keys)
            {
                var stopwatch = performanceDatas[graphNode];
                stopwatch.Stop();
                GraphNodeLoadingDurationInfo[graphNode] = stopwatch.ElapsedMilliseconds;
                CreateLog(false);
            }
            GraphNodeLoadingDurationInfo.Clear();
            performanceDatas.Clear();
        }
    }
}