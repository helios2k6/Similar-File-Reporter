using System.Collections.Generic;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class ProgramArgsProxy : Proxy
	{
		public ProgramArgsProxy(IEnumerable<string> programArgs) : base(Globals.ProgramArgsProxy)
		{
			var parserDictionary = new InputParser(programArgs).ArgCollection;

			var path = ".";
			IList<string> pathCollection;
			if (parserDictionary.TryGetValue(ProgramArgsConstants.PathArg, out pathCollection))
			{
				path = pathCollection[0] == string.Empty ? "." : pathCollection[0];
			}

			var useDoubleHash = false;
			IList<string> doubleHashCollection;
			if(parserDictionary.TryGetValue(ProgramArgsConstants.DoubleHashFlagArg, out doubleHashCollection))
			{
				useDoubleHash = true;
			}

			IList<string> blacklistCollection;
			parserDictionary.TryGetValue(ProgramArgsConstants.BlacklistArg, out blacklistCollection);

			Args = new ProgramArgs(path, useDoubleHash, blacklistCollection);
		}

		public ProgramArgs Args { get; private set; }
	}
}
