using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	public class ProgramArgs
	{
		public ProgramArgs(string path= ".", bool doubleHash=false, IEnumerable<string> blacklist = null)
		{
			Path = path;
			DoubleHash = doubleHash;
			Blacklist = blacklist;
		}

		public string Path { get; private set; }
		public bool DoubleHash { get; private set; }
		public IEnumerable<string> Blacklist { get; private set; }
	}
}
