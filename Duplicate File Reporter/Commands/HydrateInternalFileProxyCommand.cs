using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFileReporter.Commands
{
    public sealed class HydrateInternalFileProxyCommand : SimpleCommand
    {
        private IEnumerable<InternalFile> HydrateDirectory(string path, IEnumerable<string> blacklist, IEnumerable<string> fileGlobs)
        {
            return from glob in fileGlobs
                   from file in Directory.EnumerateFiles(path, glob, SearchOption.AllDirectories).Except(blacklist)
                   select new InternalFile(file);
        }

        public override void Execute(INotification notification)
        {
            var programArgsProxy = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy);
            var path = programArgsProxy.Args.Path;

            if (!Directory.Exists(path)) Globals.Fail(("Cannot find path " + path));

            var allFiles = HydrateDirectory(path, programArgsProxy.Args.Blacklist, programArgsProxy.Args.FileGlobs);
            var internalFileProxy = Facade.RetrieveProxy<InternalFileProxy>(Globals.InternalFileProxyName);

            internalFileProxy.AddFiles(allFiles);
            internalFileProxy.SealProxy();
        }
    }
}
