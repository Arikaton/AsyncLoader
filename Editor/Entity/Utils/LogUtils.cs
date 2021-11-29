using System;
using System.IO;
using UnityEngine;

namespace LoadingModule.Editor.Entity.Utils
{
    internal static class LogUtils
    {
        internal static void CreateLogFromPerformanceMeter(PerformanceMeter performanceMeter)
        {
            using (var sw = File.AppendText(Application.dataPath + "/LoadingModuleLog.txt"))
            {
                string commonInfo = $"{DateTime.Now}\nTotal loading time - {performanceMeter.TotalLoadingTime}ms\n";
                sw.Write(commonInfo);
                
                foreach (var node in performanceMeter.GraphNodeLoadingDurationInfo.Keys)
                {
                    string nodeLoadingInfo = $"{node.Step} - {performanceMeter.GraphNodeLoadingDurationInfo[node]}ms\n";
                    sw.Write(nodeLoadingInfo);
                }
                sw.Write("\n");
            }
        }
    }
}