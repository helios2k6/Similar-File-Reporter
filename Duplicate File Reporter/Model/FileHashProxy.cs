using System;
using System.Collections.Generic;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class FileHashProxy : Proxy
	{
		private readonly IDictionary<HashCode, ClusterObject> _hashToFileMap = new Dictionary<HashCode, ClusterObject>();

		private readonly object _lockObject = new object();

		private volatile bool _sealed;

		public FileHashProxy()
			: base(Globals.FileHashProxy)
		{
		}

		public void AddFileHashEntry(IMessageDigest hash, InternalFile file)
		{
			if(_sealed) throw new InvalidOperationException("Cannot add entries after proxy has been sealed");

			var hashCode = hash.GetHash();
			lock (_lockObject)
			{
				ClusterObject cluster;
				if (!_hashToFileMap.TryGetValue(hashCode, out cluster))
				{
					cluster = new ClusterObject();
					_hashToFileMap.Add(hashCode, cluster);
				}

				cluster.AddFile(file);
			}
		}

		public void SealProxy()
		{
			_sealed = true;
		}

		public IDictionary<HashCode, ClusterObject> Clusters
		{
			get
			{
				if (!_sealed) throw new InvalidOperationException("Cannot get clusters when proxy is unsealed");
				return _hashToFileMap;
			}
		}
	}
}
