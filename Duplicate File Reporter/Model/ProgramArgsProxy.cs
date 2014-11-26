using System.Collections.Generic;
using System.IO;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
    public class ProgramArgsProxy : Proxy
    {
        public ProgramArgsProxy(IEnumerable<string> programArgs)
            : base(Globals.ProgramArgsProxy)
        {
            var parser = new InputParser(programArgs);
            var parserDictionary = parser.ArgCollection;

            InvalidArgs = parser.InvalidArgs;

            var path = Directory.GetCurrentDirectory();
            IList<string> pathCollection;
            if (parserDictionary.TryGetValue(ProgramArgsConstants.PathArg, out pathCollection))
            {
                path = pathCollection.Count < 1 ? Directory.GetCurrentDirectory() : pathCollection[0];
            }

            IList<string> blacklistCollection;
            if (!parserDictionary.TryGetValue(ProgramArgsConstants.BlacklistArg, out blacklistCollection))
                blacklistCollection = new List<string>();

            var outputFile = string.Empty;
            IList<string> outputFileCollection;
            if (parserDictionary.TryGetValue(ProgramArgsConstants.OutputArg, out outputFileCollection))
                outputFile = outputFileCollection[0];

            var outputFileFormat = OutputReportType.FLAT.ToString();
            IList<string> outputFileFormatCollection;
            if (parserDictionary.TryGetValue(ProgramArgsConstants.OutputFormatArg, out outputFileFormatCollection))
                outputFileFormat = outputFileFormatCollection[0];


            //Add default blacklist stuff
            blacklistCollection.Add("thumbs.db");
            blacklistCollection.Add("Thumbs.db");
            blacklistCollection.Add("readme.txt");
            blacklistCollection.Add("read me.txt");
            blacklistCollection.Add("Help Wanted.txt");
            blacklistCollection.Add("PureMVC.DotNET.35");
            blacklistCollection.Add("SimMetrics");
            blacklistCollection.Add("DuplicateFileReporter");

            var userWantsHelp = parserDictionary.ContainsKey(ProgramArgsConstants.HelpArg);

            var useStringClusterAnalysis = parserDictionary.ContainsKey(ProgramArgsConstants.UseStringClusterAnalysisArg);

            var useFnv = parserDictionary.ContainsKey(ProgramArgsConstants.UseFnvHash);

            var useCrc32 = parserDictionary.ContainsKey(ProgramArgsConstants.UseCrc32Hash);

            Args = new ProgramArgs(path, useStringClusterAnalysis, useFnv, useCrc32, blacklistCollection, outputFile, outputFileFormat, userWantsHelp);
        }

        public ProgramArgs Args { get; private set; }

        public IEnumerable<string> InvalidArgs { get; private set; }
    }
}
