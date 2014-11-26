using System.ComponentModel;

namespace DuplicateFileReporter.Model
{
    public sealed class ReportType
    {
        private readonly string _description;

        private ReportType(string description)
        {
            _description = description;
        }

        public static readonly ReportType Crc32HashReport = new ReportType("CRC-32 Hash Report");

        public static readonly ReportType Fnv32HashReport = new ReportType("FNV-1a 32-Bit Hash Report");

        public static readonly ReportType QuickSampleReport = new ReportType("Quick Sample Report");

        public static readonly ReportType FileNameAnalysisReport = new ReportType("File Name Analysis Report");

        public override string ToString()
        {
            return _description;
        }
    }
}