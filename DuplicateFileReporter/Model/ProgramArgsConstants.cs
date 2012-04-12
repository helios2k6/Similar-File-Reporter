using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	public static class ProgramArgsConstants
	{
		public static readonly string ValidArgPrefix = "--";

		public static readonly string DoubleHashFlagArg = "--double-hash";
		public static readonly string PathArg = "--path";
		public static readonly string BlacklistArg = "--blacklist";

		public static IList<string> ValidProgramArgsVector
		{
			get
			{
				return new List<string> { DoubleHashFlagArg, PathArg, BlacklistArg };
			}
		}
	}
}