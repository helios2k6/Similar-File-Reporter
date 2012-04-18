using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	public class ReportType
	{
		private static readonly IDictionary<ReportTypeEnum, string> TypeToStringMap = new Dictionary<ReportTypeEnum, string>
		                                                                              	{
		                                                                              		{ReportTypeEnum.Crc32HashReport, "CRC-32 Hash Report"},
		                                                                              		{ReportTypeEnum.FileNameAnalysisReport, "File Name Analysis Report"},
		                                                                              		{ReportTypeEnum.Fnv32HashReport, "FNV-1a 32-bit Hash Report"},
		                                                                              	};

		public ReportType(ReportTypeEnum type)
		{
			Type = type;
		}

		public ReportTypeEnum Type { get; private set; }

		public override string ToString()
		{
			return TypeToStringMap[Type];
		}
	}
}