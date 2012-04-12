using System.Collections.Concurrent;
using System.Collections.Generic;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class FileHashProxy : Proxy
	{
		private readonly IDictionary<IMessageDigest, ClusterObject> _hashToFileMap = new ConcurrentDictionary<IMessageDigest, ClusterObject>();

		public FileHashProxy()
			: base(Globals.FileHashProxy)
		{
		}

		public void AddFileHashEntry(IMessageDigest hash, InternalFile file)
		{
			ClusterObject entry;

			if (!_hashToFileMap.TryGetValue(hash, out entry))
			{
				entry = new ClusterObject();
			}

			entry.AddFile(file);
		}

		public ClusterObject GetCluster(IMessageDigest hash)
		{
			ClusterObject entry;
			_hashToFileMap.TryGetValue(hash, out entry);

			return entry;
		}
	}
}
