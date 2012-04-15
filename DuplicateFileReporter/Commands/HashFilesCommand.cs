using System.IO;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
	public class HashFilesCommand : SimpleCommand
	{
		public void HashFileFnv(object arg)
		{
			var file = arg as InternalFile;

			if(file == null)
			{
				Facade.SendNotification(Globals.LogErrorNotification, "Unable to hash file. Could not cast to internal file");
				return;
			}

			SendNotification(Globals.LogInfoNotification, "Using FNV-1a hash on " + file);

			var digest = new FnvMessageDigest();

			using(Stream stream = File.Open(file.GetPath(), FileMode.Open))
			{
				digest.Update(stream);
			}

			var hashProxy = Facade.RetrieveProxy(Globals.FileHashProxy) as FileHashProxy;

			if(hashProxy == null) Globals.Fail("Could not get Hash Proxy");

			hashProxy.AddFileHashEntry(digest, file);
		}

		public void HashFileCrc32(object arg)
		{
			var file = arg as InternalFile;

			if (file == null)
			{
				Facade.SendNotification(Globals.LogErrorNotification, "Unable to hash file. Could not cast to internal file");
				return;
			}

			SendNotification(Globals.LogInfoNotification, "Using CRC-32 hash on " + file);

			var digest = new Crc32MessageDigest();

			using (Stream stream = File.Open(file.GetPath(), FileMode.Open))
			{
				digest.ComputeHash(stream);
			}

			var hashProxy = Facade.RetrieveProxy(Globals.FileHashProxy) as FileHashProxy;

			if (hashProxy == null) Globals.Fail("Could not get Hash Proxy");

			hashProxy.AddFileHashEntry(digest, file);
		}

		public override void Execute(INotification notification)
		{
			var internalFileProxy = Facade.RetrieveProxy(Globals.InternalFileProxyName) as InternalFileProxy;
			var argProxy = Facade.RetrieveProxy(Globals.ProgramArgsProxy) as ProgramArgsProxy;
			var threadPoolProxy = Facade.RetrieveProxy(Globals.ThreadPoolProxy) as ThreadPoolProxy;

			if(internalFileProxy == null || argProxy == null || threadPoolProxy == null)
				Globals.Fail("Could not cast InternalFileProxy or ProgramArgsProxy or ThreadPoolProxy");

			var listOfFiles = internalFileProxy.GetListOfFiles();
			var threadPool = threadPoolProxy.Threadpool;

			foreach(var f in listOfFiles)
			{
				if (argProxy.Args.UseFnvHash)
					threadPool.SubmitAction(HashFileFnv, f);

				if(argProxy.Args.UseCrc32Hash)
					threadPool.SubmitAction(HashFileCrc32, f);
			}
		}
	}
}
