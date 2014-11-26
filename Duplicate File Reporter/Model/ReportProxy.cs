
using System.Collections.Concurrent;
using System.Collections.Generic;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class ReportProxy : Proxy
	{
		private readonly ConcurrentQueue<Report> _reports = new ConcurrentQueue<Report>();

		public ReportProxy() : base(Globals.ReportProxy)
		{
			
		}

		public void AddReport(Report report)
		{
			_reports.Enqueue(report);
		}

		public bool GetReport(out Report report)
		{
			return _reports.TryDequeue(out report);
		}

		public IEnumerable<Report> Reports { get { return _reports; } }
	}
}
