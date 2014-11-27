using System;
using System.IO;
using System.Text.RegularExpressions;

namespace DuplicateFileReporter.Model
{
    public sealed class InternalFile
    {
        private const string Deliminators = @"[~`_-]";

        private const string BracketGroups = @"(\[\w*\d*\])";

        private const string ParenthesisGroups = @"(\(\w*\d*\))";

        private readonly string _filePath;
        private readonly string _cleanFileName;

        public InternalFile(string filePath)
        {
            _filePath = filePath;
            _cleanFileName = ProcessFileName(filePath);
        }

        private static string ProcessFileName(string fileName)
        {
            fileName = Regex.Replace(fileName, Deliminators, string.Empty);

            fileName = Regex.Replace(fileName, BracketGroups, string.Empty);

            fileName = Regex.Replace(fileName, ParenthesisGroups, string.Empty);

            fileName = Regex.Replace(fileName, " ", string.Empty);

            return fileName;
        }

        public string FileName
        {
            get { return Path.GetFileName(_filePath); }
        }

        public string FilePath
        {
            get { return _filePath; }
        }

        public override string ToString()
        {
            return FilePath;
        }

        public string CleanFileName
        {
            get { return _cleanFileName; }
        }
    }
}
