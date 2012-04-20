using System;
using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	public class ProgramArgs
	{
		public ProgramArgs(string path, 
			bool useStringClusterAnalysis, 
			bool useFnvHash, 
			bool useMd5Hash, 
			IEnumerable<string> blacklist, 
			string outputFile,
			string outputFileFormat,
			bool help)
		{
			Path = path;
			UseFnvHash = useFnvHash;
			UseCrc32Hash = useMd5Hash;
			UseStringClusterAnalysis = useStringClusterAnalysis;
			Blacklist = blacklist;
			OutputFile = outputFile;
			OutputFileFormat = (OutputReportType)Enum.Parse(typeof(OutputReportType), outputFileFormat.ToUpper());
			Help = help;
		}

		public string Path { get; private set; }
		public bool UseFnvHash { get; private set; }
		public bool UseCrc32Hash { get; private set; }
		public bool UseStringClusterAnalysis { get; private set; }
		public IEnumerable<string> Blacklist { get; private set; }
		public string OutputFile { get; private set; }
		public OutputReportType OutputFileFormat { get; private set; } 
		public bool Help { get; private set; }
	}
}
