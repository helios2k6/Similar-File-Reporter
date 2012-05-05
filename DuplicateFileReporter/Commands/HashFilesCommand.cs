using System.Collections.Generic;
using System.IO;
using System.Threading;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Threading.Tasks;
using System;

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

			digest.DoFinal();

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

			if(internalFileProxy == null || argProxy == null)
				Globals.Fail("Could not cast InternalFileProxy or ProgramArgsProxy or ThreadPoolProxy");

			var listOfFiles = internalFileProxy.GetListOfFiles();

			var listOfTasks = new List<Task>();

			foreach(var f in listOfFiles)
			{
				if (argProxy.Args.UseFnvHash){
					listOfTasks.Add(Task.Factory.StartNew(HashFileFnv, f));
				}

				if (argProxy.Args.UseCrc32Hash)
				{
					listOfTasks.Add(Task.Factory.StartNew(HashFileCrc32, f));
				}
			}

			foreach (var f in listOfTasks)
			{
				f.Wait();
			}

			var fileHashProxy = Facade.RetrieveProxy(Globals.FileHashProxy) as FileHashProxy;

			fileHashProxy.SealProxy();
		}
	}
}
