namespace DuplicateFileReporter.Model
{
	public class Report
	{
		private static volatile int _idCounter;

		public static int GetNextId()
		{
			return _idCounter++;
		}

		public int Id { get; set; }

		public ReportTypeEnum Type { get; set; }

		public ClusterObject Cluster { get; set; }
	}

}
