namespace DuplicateFileReporter.Model
{
	public class Report
	{
		private static volatile int _idCounter;

		public Report(ReportType type, ClusterObject cluster)
		{
			Id = _idCounter++;
			Type = type;
			Cluster = cluster;
		}

		public int Id { get; private set; }

		public ReportType Type { get; private set; }

		public ClusterObject Cluster { get; private set; }

		
	}
}
