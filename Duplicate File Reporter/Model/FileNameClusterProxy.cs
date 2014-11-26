using System.Collections.Concurrent;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class FileNameClusterProxy : Proxy
	{
		public FileNameClusterProxy() : base(Globals.FileNameClusterProxy)
		{
			FileNameClusters = new ConcurrentBag<ClusterObject>();
		}

		public void AddCluster(ClusterObject cluster)
		{
			FileNameClusters.Add(cluster);
		}

		public ConcurrentBag<ClusterObject> FileNameClusters { get; private set; }
	}
}
