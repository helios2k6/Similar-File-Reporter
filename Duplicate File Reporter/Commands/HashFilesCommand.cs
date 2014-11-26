using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace DuplicateFileReporter.Commands
{
    public sealed class HashFilesCommand : SimpleCommand
    {
        private Task HashFileFnvAsync(InternalFile file)
        {
            return Task.Factory.StartNew(() =>
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
            });
        }

        private Task HashFileCrc32Async(InternalFile file)
        {
            return Task.Factory.StartNew(() =>
            {
                SendNotification(Globals.LogInfoNotification, "Using CRC-32 hash on " + file);

                var digest = new Crc32MessageDigest();
                using (Stream stream = File.OpenRead(file.GetPath()))
                {
                    digest.ComputeHash(stream);
                }

                var hashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
                hashProxy.AddFileHashEntry(digest, file);
            });
        }

        private void AddQuickSampleDigest(QuickSampleMessageDigest digest)
        {
            var hashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
            hashProxy.AddFileHashEntry(digest, digest.File);
        }

        private IEnumerable<QuickSampleMessageDigest> GenerateSampleDigests()
        {
            SendNotification(Globals.LogInfoNotification, "Sampling Files");
            var fileProxy = Facade.RetrieveProxy<InternalFileProxy>(Globals.InternalFileProxyName);
            var files = fileProxy.GetListOfFiles();

            return files.Select(t => new QuickSampleMessageDigest(t));
        }

        private IEnumerable<QuickSampleMessageDigest> FilterUniqueSampleDigests(IEnumerable<QuickSampleMessageDigest> digests)
        {
            var groups = new Dictionary<HashCode, ISet<QuickSampleMessageDigest>>();

            foreach(var digest in digests)
            {
                SendNotification(Globals.LogInfoNotification, "Sampling file: " + digest.File.GetCleanedFileName());

                ISet<QuickSampleMessageDigest> group;
                if(groups.TryGetValue(digest.GetHash(), out group) == false)
                {
                    group = new HashSet<QuickSampleMessageDigest>();
                    groups[digest.GetHash()] = group;
                }

                group.Add(digest);
            }

            return from g in groups
                   where g.Value.Count > 1
                   from digest in g.Value
                   select digest;
        }

        private void ProcessFiles(IEnumerable<QuickSampleMessageDigest> digests)
        {
            var listOfTasks = new List<Task>();
            var args = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy).Args;

            foreach (var digest in digests)
            {
                if (args.UseQuickSampleHash)
                {
                    AddQuickSampleDigest(digest);
                }

                if (args.UseFnvHash)
                {
                    listOfTasks.Add(HashFileFnvAsync(digest.File));
                }

                if (args.UseCrc32Hash)
                {
                    listOfTasks.Add(HashFileCrc32Async(digest.File));
                }
            }

            Task.WaitAll(listOfTasks.ToArray());
        }

        public override void Execute(INotification notification)
        {
            var argProxy = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy);
            if (argProxy.Args.UseCrc32Hash || argProxy.Args.UseFnvHash || argProxy.Args.UseQuickSampleHash)
            {
                var sampleDigests = GenerateSampleDigests();
                var suspectedDuplicateFileSets = FilterUniqueSampleDigests(sampleDigests);
                ProcessFiles(suspectedDuplicateFileSets);
            }

            var fileHashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
            fileHashProxy.SealProxy();
        }
    }
}
