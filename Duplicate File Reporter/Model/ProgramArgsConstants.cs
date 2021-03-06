using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
    public static class ProgramArgsConstants
    {
        public static readonly string ValidArgPrefix = "--";

        public static readonly string PathArg = "--path";
        public static readonly string BlacklistArg = "--blacklist";
        public static readonly string HelpArg = "--help";
        public static readonly string UseStringClusterAnalysisArg = "--use-name-analysis";
        public static readonly string UseFnvHash = "--use-fnv-hash";
        public static readonly string UseQuickSampleHash = "--use-sample-hash";
        public static readonly string OutputArg = "--output";
        public static readonly string OutputFormatArg = "--output-format";
        public static readonly string FileGlobsArg = "--file-globs";

        public static readonly string PathArgHelpString = "Folder path you want to analyze. (Default: Current directory)";
        public static readonly string BlacklistArgHelpString = "Files you do not want to analyze";
        public static readonly string HelpArgHelpString = "Print this message";
        public static readonly string UseStringClusterAnalysisArgHelpString = "Use file name similarity to cluster possible file duplicates";
        public static readonly string UseFnvHashHelpString = "Use the FNV-1a hash to detect file duplicates";
        public static readonly string UseQuickSampleHashString = "Use a quick sampling hash to detect file duplicates";
        public static readonly string OutputArgHelpString = "Output file to print results to";
        public static readonly string FileGlobsArgHelpString = "The file globs you wish to capture. (Default: All file types)";

        public static readonly string OutputFormatArgHelpString = "Format of the report. Valid inputs: XML, JSON, or flat (Default: Flat)";

        public static IList<string> ValidProgramArgsVector
        {
            get
            {
                return new List<string> 
                { 
                    PathArg, 
                    BlacklistArg, 
                    HelpArg, 
                    UseStringClusterAnalysisArg,
                    UseFnvHash,
                    UseQuickSampleHash,
                    OutputArg,
                    OutputFormatArg,
                    FileGlobsArg,
                };
            }
        }

        public static IEnumerable<KeyValuePair<string, string>> ProgramArgsHelpMap
        {
            get
            {
                return new Dictionary<string, string>
                        {
                            {PathArg, PathArgHelpString},
                            {BlacklistArg, BlacklistArgHelpString},
                            {UseStringClusterAnalysisArg, UseStringClusterAnalysisArgHelpString},
                            {UseFnvHash, UseFnvHashHelpString},
                            {UseQuickSampleHash, UseQuickSampleHashString},
                            {HelpArg, HelpArgHelpString},
                            {OutputArg, OutputArgHelpString},
                            {OutputFormatArg, OutputFormatArgHelpString},
                            {FileGlobsArg, FileGlobsArgHelpString},
                        };
            }
        }
    }
}