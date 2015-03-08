using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace DuplicateFileReporter.Commands
{
    public sealed class HashFilesCommand : SimpleCommand
    {
        private void HashFileFnv(InternalFile file)
        {
            var digest = new FnvMessageDigest();
            using (Stream stream = File.OpenRead(file.FilePath))
            {
                digest.Update(stream);
            }

            digest.DoFinal();
            Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy).AddFileHashEntry(digest, file);
        }

        private void AddQuickSampleDigest(QuickSampleMessageDigest digest)
        {
            var hashProxy = Facade.RetrieveProxy<FileHashProxy>(Globals.FileHashProxy);
            hashProxy.AddFileHashEntry(digest, digest.File);
        }

        private IEnumerable<QuickSampleMessageDigest> GenerateSampleDigests()
        {
            var fileProxy = Facade.RetrieveProxy<InternalFileProxy>(Globals.InternalFileProxyName);
            var files = fileProxy.GetListOfFiles();

            return files.Select(t => new QuickSampleMessageDigest(t));
        }

        private IEnumerable<QuickSampleMessageDigest> FilterUniqueSampleDigests(IEnumerable<QuickSampleMessageDigest> digests)
        {
            var groups = new Dictionary<HashCode, ISet<QuickSampleMessageDigest>>();

            long hashedFiles = 0;
            long numFiles = digests.LongCount();

            SendNotification(Globals.LogInfoNotification, string.Format("Sampling {0} file(s)", numFiles));

            Parallel.ForEach(digests, digest =>
            {
                long incrementedValue = Interlocked.Increment(ref hashedFiles);

                digest.GetHash();

                if (incrementedValue % 20 == 0)
                {
                    SendNotification(Globals.LogInfoNotification, string.Format("Sampled {0} files. {1} remaining", incrementedValue, numFiles - incrementedValue));
                }
            });

            foreach(var digest in digests)
            {
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
            var args = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy).Args;

            long hashedFiles = 0;
            long numFiles = digests.LongCount();
            SendNotification(Globals.LogInfoNotification, string.Format("Hashing {0} file(s)", numFiles));

            foreach (var digest in digests)
            {
                if (args.UseQuickSampleHash)
                {
                    AddQuickSampleDigest(digest);
                }

                if (args.UseFnvHash)
                {
                    HashFileFnv(digest.File);
                }

                hashedFiles++;

                if(hashedFiles % 5 == 0)
                {
                    SendNotification(Globals.LogInfoNotification, string.Format("{0} file(s) hashed. {1} file(s) remaining", hashedFiles, numFiles - hashedFiles));
                }
            }

            SendNotification(Globals.LogInfoNotification, "Done hashing!");
        }

        public override void Execute(INotification notification)
        {
            var argProxy = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy);
            if (argProxy.Args.UseFnvHash || argProxy.Args.UseQuickSampleHash)
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
