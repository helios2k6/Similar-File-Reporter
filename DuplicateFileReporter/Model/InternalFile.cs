using System;
using System.Text.RegularExpressions;

namespace DuplicateFileReporter.Model
{
	public class InternalFile
	{
		private const string Deliminators = @"[~`_-]";

		private const string BracketGroups = @"(\[\w*\d*\])";

		private const string ParenthesisGroups = @"(\(\w*\d*\))";

		private readonly Uri _uri;

		private string _cleanFileName;

		public InternalFile(string uri)
		{
			_uri = new Uri(uri);
			InitCleanFileName();
		}

		public InternalFile(Uri uri)
		{
			_uri = uri;
			InitCleanFileName();
		}

		private void InitCleanFileName()
		{
			_cleanFileName = GetFileName();

			_cleanFileName = Regex.Replace(_cleanFileName, Deliminators, string.Empty);

			_cleanFileName = Regex.Replace(_cleanFileName, BracketGroups, string.Empty);

			_cleanFileName = Regex.Replace(_cleanFileName, ParenthesisGroups, string.Empty);

			_cleanFileName = Regex.Replace(_cleanFileName, " ", string.Empty);
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public string GetFileName()
		{
			var segments = _uri.Segments;
			return Uri.UnescapeDataString(segments[segments.Length - 1]);
		}

		public string GetPath()
		{
			return Uri.LocalPath;
		}

		public override string ToString()
		{
			return GetPath();
		}

		public string GetCleanedFileName()
		{
			return _cleanFileName;
		}
	}
}
