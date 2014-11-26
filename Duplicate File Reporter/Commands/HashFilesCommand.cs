using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DuplicateFileReporter.Commands
{
    public sealed class HashFilesCommand : SimpleCommand
    {
        private void HashFileFnv(InternalFile file)
        {
            SendNotification(Globals.LogInfoNotification, "Using FNV-1a hash on " + file);

            var digest = new FnvMessageDigest();
            using (Stream stream = File.OpenRead(file.GetPath()))
            {
                digest.Update(stream);
            }

            digest.DoFinal();

            var hashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
            hashProxy.AddFileHashEntry(digest, file);
        }

        private void HashFileCrc32(InternalFile file)
        {
            SendNotification(Globals.LogInfoNotification, "Using CRC-32 hash on " + file);

            var digest = new Crc32MessageDigest();
            using (Stream stream = File.OpenRead(file.GetPath()))
            {
                digest.ComputeHash(stream);
            }

            var hashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
            hashProxy.AddFileHashEntry(digest, file);
        }

        private void ProcessFnvHash()
        {
            var argProxy = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy);
            if (!argProxy.Args.UseFnvHash) return;

            var internalFileProxy = Facade.RetrieveProxy<InternalFileProxy>(Globals.InternalFileProxyName);
            var listOfFiles = internalFileProxy.GetListOfFiles();

            Parallel.ForEach(listOfFiles, e => HashFileFnv(e));
        }

        private void ProcessCrc32Hash()
        {
            var argProxy = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy);
            if (!argProxy.Args.UseCrc32Hash) return;

            var internalFileProxy = Facade.RetrieveProxy<InternalFileProxy>(Globals.InternalFileProxyName);
            var listOfFiles = internalFileProxy.GetListOfFiles();

            Parallel.ForEach(listOfFiles, e => HashFileCrc32(e));
        }

        private IEnumerable<ISet<InternalFile>> GenerateSuspectedClusters()
        {
            return null;
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

            var fileHashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
            fileHashProxy.SealProxy();
        }
    }
}
