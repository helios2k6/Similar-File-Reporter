
using System;
using System.Text;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
	public class OutputReportsCommand : SimpleCommand
	{
		public override void Execute(INotification notification)
		{
			var reportProxy = Facade.RetrieveProxy(Globals.ReportProxy) as ReportProxy;
			if(reportProxy == null) Globals.Fail("Could not get ReportProxy");

			var builder = new StringBuilder();

			builder.Append("Begin Reports Dump").AppendLine();

			foreach(var r in reportProxy.Reports)
			{
				builder.Append("****Report ").Append(r.Id).Append("***").AppendLine();
				builder.Append("Report Type: ").Append(r.Type).AppendLine();

				foreach(var f in r.Cluster.Files)
				{
					builder.Append(f).AppendLine();
				}
				builder.Append("===End of Report===").AppendLine().AppendLine();
			}

			builder.Append("End of Report Dump");
			Console.WriteLine(builder.ToString());
		}
	}
}
