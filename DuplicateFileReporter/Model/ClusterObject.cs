using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	public class ClusterObject
	{
		private readonly ConcurrentBag<InternalFile> _files = new ConcurrentBag<InternalFile>();

		public void AddFile(InternalFile file)
		{
			_files.Add(file);
		}

		public IEnumerable<InternalFile> Files
		{
			get { return _files; }
		}

		public int Count { get { return _files.Count; } }
	}
}
