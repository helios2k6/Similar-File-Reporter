using System.Linq;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
    public class GenerateClusterAnalysisReportCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            var fileNameClusterProxy = Facade.RetrieveProxy(Globals.FileNameClusterProxy) as FileNameClusterProxy;
            if (fileNameClusterProxy == null) Globals.Fail("Could not get FileNameClusterProxy");

            var clusters = fileNameClusterProxy.FileNameClusters;

            var groups = (from c in clusters
                          where c.Count > 1
                          select c).ToList();

            SendNotification(Globals.LogInfoNotification, "Analyzing clusters based on file-names. There are " + groups.Count + " clusters to analyze");

            var reportProxy = Facade.RetrieveProxy(Globals.ReportProxy) as ReportProxy;
            if (reportProxy == null) Globals.Fail("Could not get ReportProxy");

            foreach (var g in groups)
            {
                var report = new Report { Id = Report.GetNextId(), Type = ReportTypeEnum.FileNameAnalysisReport, Cluster = g };
                reportProxy.AddReport(report);
            }
        }
    }
}
