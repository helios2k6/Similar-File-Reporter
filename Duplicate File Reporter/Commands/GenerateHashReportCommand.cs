using System.Linq;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System;

namespace DuplicateFileReporter.Commands
{
    public sealed class GenerateHashReportCommand : SimpleCommand
    {
        public override void Execute(INotification notification)
        {
            var fileHashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
            var clusters = fileHashProxy.Clusters;
            var groups = (from c in clusters
                          where c.Value.Count > 1
                          select c).ToList();

            var reportProxy = Facade.RetrieveProxy<ReportProxy>(Globals.ReportProxy);
            SendNotification(Globals.LogInfoNotification, "Analyzing clusters based on hash codes for files. There are " + groups.Count() + " clusters to Analyze");

            foreach (var g in groups)
            {
                var hashCode = g.Key;
                var report = new Report { Id = Report.GetNextId(), Cluster = g.Value };

                switch (hashCode.HashCodeType)
                {
                    case HashCodeType.Crc32Hash:
                        report.ReportType = ReportType.Crc32HashReport;
                        break;
                    case HashCodeType.Fnv1A32Hash:
                        report.ReportType = ReportType.Fnv32HashReport;
                        break;
                    case HashCodeType.SampleHash:
                        report.ReportType = ReportType.QuickSampleReport;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown hash report type");
                }

                reportProxy.AddReport(report);
            }
        }
    }
}
