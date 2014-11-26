using System.ComponentModel;

namespace DuplicateFileReporter.Model
{
    public enum ReportTypeEnum
    {
        [Description("CRC-32 Hash Report")]
        Crc32HashReport,
        [Description("FNV-1a 32-Bit Hash Report")]
        Fnv32HashReport,
        [Description("File Name Analysis Report")]
        FileNameAnalysisReport
    }
}