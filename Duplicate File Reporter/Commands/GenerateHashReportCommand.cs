
using System.Linq;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
	public class GenerateHashReportCommand : SimpleCommand
	{
		public override void Execute(INotification notification)
		{
			var fileHashProxy = Facade.RetrieveProxy(Globals.FileHashProxy) as FileHashProxy;
			if (fileHashProxy == null) Globals.Fail("Could not get FileNameClusterProxy");

			var clusters = fileHashProxy.Clusters;

			var groups = (from c in clusters
			             where c.Value.Count > 1
			             select c).ToList();

			var reportProxy = Facade.RetrieveProxy(Globals.ReportProxy) as ReportProxy;
			if(reportProxy == null) Globals.Fail("Could not get ReportProxy");

			SendNotification(Globals.LogInfoNotification, "Analyzing clusters based on hash codes for files. There are " + groups.Count() + " clusters to Analyze");

			foreach(var g in groups)
			{
				var hashCode = g.Key;
				var report = new Report {Id = Report.GetNextId(),Cluster = g.Value};

				switch (hashCode.HashCodeType)
				{
					case HashCodeType.Crc32Hash:
						report.Type = ReportTypeEnum.Crc32HashReport;
						break;
					case HashCodeType.Fnv1A32Hash:
						report.Type = ReportTypeEnum.Fnv32HashReport;
						break;
				}

				reportProxy.AddReport(report);
			}
		}
	}
}
