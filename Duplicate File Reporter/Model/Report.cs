using System.Text;
namespace DuplicateFileReporter.Model
{
    public sealed class Report
    {
        private static volatile int _idCounter;

        public static int GetNextId()
        {
            return _idCounter++;
        }

        public int Id { get; set; }

        public ReportType ReportType { get; set; }

        public ClusterObject Cluster { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("=Report=");
            builder.AppendLine("ID: " + Id);
            builder.AppendLine("Type: " + ReportType);
            builder.AppendLine("==Analysis==");
            builder.Append(Cluster.ToString());
            builder.AppendLine("=End of Report=");

            return builder.ToString();
        }
    }

}
