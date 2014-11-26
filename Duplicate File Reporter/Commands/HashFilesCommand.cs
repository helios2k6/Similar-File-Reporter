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

            if (file == null)
            {
                Facade.SendNotification(Globals.LogErrorNotification, "Unable to hash file. Could not cast to internal file");
                return;
            }

            SendNotification(Globals.LogInfoNotification, "Using FNV-1a hash on " + file);

            var digest = new FnvMessageDigest();

            using (Stream stream = File.OpenRead(file.GetPath()))
            {
                digest.Update(stream);
            }

            digest.DoFinal();

            var hashProxy = Facade.RetrieveProxy(Globals.FileHashProxy) as FileHashProxy;

            if (hashProxy == null) Globals.Fail("Could not get Hash Proxy");

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

            using (Stream stream = File.OpenRead(file.GetPath()))
            {
                digest.ComputeHash(stream);
            }

            var hashProxy = Facade.RetrieveProxy(Globals.FileHashProxy) as FileHashProxy;

            if (hashProxy == null) Globals.Fail("Could not get Hash Proxy");

            hashProxy.AddFileHashEntry(digest, file);
        }

        private void ProcessFnvHash()
        {
            var argProxy = Facade.RetrieveProxy(Globals.ProgramArgsProxy) as ProgramArgsProxy;
            if (!argProxy.Args.UseFnvHash) return;

            var internalFileProxy = Facade.RetrieveProxy(Globals.InternalFileProxyName) as InternalFileProxy;
            var listOfFiles = internalFileProxy.GetListOfFiles();

            Parallel.ForEach(listOfFiles, e => HashFileFnv(e));
        }

        private void ProcessCrc32Hash()
        {
            var argProxy = Facade.RetrieveProxy(Globals.ProgramArgsProxy) as ProgramArgsProxy;
            if (!argProxy.Args.UseCrc32Hash) return;

            var internalFileProxy = Facade.RetrieveProxy(Globals.InternalFileProxyName) as InternalFileProxy;
            var listOfFiles = internalFileProxy.GetListOfFiles();

            Parallel.ForEach(listOfFiles, e => HashFileCrc32(e));
        }

        public override void Execute(INotification notification)
        {
            var listOfTasks = new List<Task> 
            { 
                Task.Factory.StartNew(ProcessFnvHash),
                Task.Factory.StartNew(ProcessCrc32Hash)
            };

            foreach (var t in listOfTasks)
            {
                t.Wait();
            }

            var fileHashProxy = Facade.RetrieveProxy(Globals.FileHashProxy) as FileHashProxy;

            fileHashProxy.SealProxy();
        }
    }
}
