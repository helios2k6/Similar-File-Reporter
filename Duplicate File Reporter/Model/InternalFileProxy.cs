using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class InternalFileProxy : Proxy
	{
		private readonly ConcurrentBag<InternalFile> _internalFiles = new ConcurrentBag<InternalFile>();
		private volatile bool _sealed;

		public InternalFileProxy()
			: base(Globals.InternalFileProxyName)
		{
		}

		public void AddFiles(IEnumerable<InternalFile> files)
		{
			foreach(var i in files)
			{
				_internalFiles.Add(i);
			}
		}

		public void AddFile(InternalFile file)
		{
			if(_sealed) throw new InvalidOperationException("Cannot add files when Internal File Proxy is sealed");

			_internalFiles.Add(file);
		}

		public IEnumerable<InternalFile> GetListOfFiles()
		{
			if(!_sealed) throw new InvalidOperationException("Cannot get list of files when Internal File Proxy is not sealed");

			return _internalFiles;
		}

		public void SealProxy()
		{
			_sealed = true;
		}

		public bool IsSealed()
		{
			return _sealed;
		}
	}
}
