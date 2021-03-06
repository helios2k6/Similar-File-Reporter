﻿using System;
using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
    public sealed class ProgramArgs
    {
        public ProgramArgs(
            string path,
            bool useStringClusterAnalysis,
            bool useFnvHash,
            bool useQuickSampleHash,
            IEnumerable<string> blacklist,
            string outputFile,
            string outputFileFormat,
            bool help,
            IEnumerable<string> fileGlobs)
        {
            Path = path;
            UseFnvHash = useFnvHash;
            UseStringClusterAnalysis = useStringClusterAnalysis;
            UseQuickSampleHash = useQuickSampleHash;
            Blacklist = blacklist;
            OutputFile = outputFile;
            OutputFileFormat = (OutputReportType)Enum.Parse(typeof(OutputReportType), outputFileFormat.ToUpper());
            Help = help;
            FileGlobs = fileGlobs;
        }

        public string Path { get; private set; }
        public bool UseFnvHash { get; private set; }
        public bool UseStringClusterAnalysis { get; private set; }
        public bool UseQuickSampleHash { get; private set; }
        public IEnumerable<string> Blacklist { get; private set; }
        public string OutputFile { get; private set; }
        public OutputReportType OutputFileFormat { get; private set; }
        public bool Help { get; private set; }
        public IEnumerable<string> FileGlobs { get; private set; }
    }
}
