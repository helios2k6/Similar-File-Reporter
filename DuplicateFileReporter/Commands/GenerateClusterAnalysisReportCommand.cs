using System;
using System.Linq;
using System.Text;
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

			SendNotification(Globals.LogInfoNotification, "Analyzing Groups. There are " + groups.Count + " clusters to analyze");

			var reportNumber = 1;
			if (groups.Count > 0)
			{
				foreach (var g in groups)
				{
					var builder = new StringBuilder();

					builder.Append("===").Append("Report ").Append(reportNumber).Append("===").AppendLine();
					reportNumber++;

					foreach (var f in g.Files)
					{
						builder.Append(f).AppendLine();
					}

					builder.AppendLine().AppendLine();

					Console.WriteLine(builder.ToString());
				}
			}else
			{
				Console.WriteLine("No duplicate files were detected");
			}
		}
	}
}
