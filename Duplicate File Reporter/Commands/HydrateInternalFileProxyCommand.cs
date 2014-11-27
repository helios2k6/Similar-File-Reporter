using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
    public sealed class HydrateInternalFileProxyCommand : SimpleCommand
    {
        private IEnumerable<InternalFile> HydrateDirectory(string path, IEnumerable<string> blacklist)
        {
            var files = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Except(blacklist);
            return files.Select(t => new InternalFile(t));
        }

        public override void Execute(INotification notification)
        {
            var programArgsProxy = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy);
            var path = programArgsProxy.Args.Path;

            if (!Directory.Exists(path)) Globals.Fail(("Cannot find path " + path));

            var allFiles = HydrateDirectory(path, programArgsProxy.Args.Blacklist.ToList());
            var internalFileProxy = Facade.RetrieveProxy<InternalFileProxy>(Globals.InternalFileProxyName);

            internalFileProxy.AddFiles(allFiles);
            internalFileProxy.SealProxy();
        }
    }
}
