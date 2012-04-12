using System;

namespace DuplicateFileReporter.Model
{
	public class InternalFile
	{
		private readonly Uri _uri;

		public InternalFile(string uri)
		{
			_uri = new Uri(uri);
		}

		public InternalFile(Uri uri)
		{
			_uri = uri;
		}

		public Uri Uri
		{
			get { return _uri; }
		}

		public string GetFileName()
		{
			var segments = _uri.Segments;
			return segments[segments.Length - 1];
		}

		public string GetPath()
		{
			return Uri.LocalPath;
		}
	}
}
