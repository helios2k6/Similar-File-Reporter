using System.Collections.Concurrent;
using System.Collections.Generic;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class FileNameClusterProxy : Proxy
	{
		private readonly ConcurrentBag<ClusterObject> _fileNameCluster = new ConcurrentBag<ClusterObject>();

		public FileNameClusterProxy() : base(Globals.FileNameClusterProxy)
		{
		}

		public void AddCluster(ClusterObject cluster)
		{
			_fileNameCluster.Add(cluster);
		}

		public IEnumerable<ClusterObject> FileNameCluster
		{
			get { return _fileNameCluster; }
		}
	}
}
