using System.Collections.Generic;
using System.Linq;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
    public class AnalyzeNamesCommand : SimpleCommand
    {
        private const double MagicMembershipThresholdCoefficient = 0.95;
        private const double MagicEarlyTerminationCoefficient = MagicMembershipThresholdCoefficient - 0.20;

        private bool EvaluateFileMembershipInCluster(InternalFile file, ClusterObject cluster, out double result)
        {
            var stringComparisonToolsProxy = Facade.RetrieveProxy(Globals.StringComparisonToolsProxy) as StringComparisonToolsProxy;

            if (stringComparisonToolsProxy == null) Globals.Fail("Could not cast StringComparisonToolsProxy");

            var avg = 0.0;
            var count = 0;

            foreach (var f in cluster.Files)
            {
                var currentAvg = stringComparisonToolsProxy.CompareSimilarities(file.GetCleanedFileName(), f.GetCleanedFileName());

                if (currentAvg < MagicEarlyTerminationCoefficient)
                {
                    result = -1;
                    return false;
                }

                avg += currentAvg;
                count++;
            }

            result = avg / count;

            return result <= MagicMembershipThresholdCoefficient;
        }

        public override void Execute(INotification notification)
        {
            var internalFileProxy = Facade.RetrieveProxy(Globals.InternalFileProxyName) as InternalFileProxy;
            var fileNameClusterProxy = Facade.RetrieveProxy(Globals.FileNameClusterProxy) as FileNameClusterProxy;

            if (internalFileProxy == null || fileNameClusterProxy == null)
                Globals.Fail("Could not cast InternalFileProxy or FileNameClusterProxy");

            /*
             * Cluster Analysis Proceedure
             * 1. For each file, f, figure out which cluster it belongs in
             * 2. If f does not belong to any cluster, create one
             */
            var files = internalFileProxy.GetListOfFiles().ToList();

            SendNotification(Globals.LogInfoNotification, "Beginning file clustering analysis. There are " + files.Count + " files to analyze");

            foreach (var f in files)
            {
                //Analyze File f
                var clusters = fileNameClusterProxy.FileNameClusters;

                var possibleClusters = new Dictionary<ClusterObject, double>();

                //Cycle through clusters
                foreach (var c in clusters)
                {
                    double result;

                    if (!EvaluateFileMembershipInCluster(f, c, out result)) continue;

                    SendNotification(Globals.LogInfoNotification, "Adding " + f.GetFileName() + " to Cluster " + c.Id);
                    possibleClusters.Add(c, result);
                }

                //See if we have any clusters
                if (possibleClusters.Count > 0)
                {
                    //Figure out which cluster has the highest double
                    var highestRatedCluster = default(ClusterObject);
                    var lastRating = 0.0;

                    foreach (var c in possibleClusters)
                    {
                        if (lastRating >= c.Value) continue;

                        highestRatedCluster = c.Key;
                        lastRating = c.Value;
                    }

                    highestRatedCluster.AddFile(f);
                }
                else
                {
                    var co = new ClusterObject();
                    co.AddFile(f);
                    fileNameClusterProxy.AddCluster(co);
                }
            }
        }
    }
}
