using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
	public class HydrateInternalFileProxyCommand : SimpleCommand
	{
		private const string FileUriPrefix = "file:///";

		private IEnumerable<InternalFile> HydrateDirectory(string path, IEnumerable<string> blacklist)
		{
			//Get all files in this directory
			var files = (from f in Directory.GetFiles(path)
			             let result = (from b in blacklist
			                           where f.Contains(b)
			                           select b).Count()
			             where result == 0
			             select f).ToList();

			SendNotification(Globals.LogInfoNotification, "Adding: " + path + " to search path");

			//Hydrate the proxy
			var internalFiles = (from f in files
								 select new InternalFile(new Uri(FileUriPrefix + f))).ToList();

			//Hydrate all subdirectories
			var subDirectories = Directory.GetDirectories(path);

			foreach (var sd in subDirectories)
			{
				var subdirectoryFiles = HydrateDirectory(sd, blacklist);

				internalFiles.AddRange(subdirectoryFiles);
			}

			return internalFiles;
		}

		public override void Execute(INotification notification)
		{
			var programArgsProxy = Facade.RetrieveProxy(Globals.ProgramArgsProxy) as ProgramArgsProxy;
			if (programArgsProxy == null) Globals.Fail("Cannot hydrate files. Couldn't cast ProgramArgsProxy");

			var path = programArgsProxy.Args.Path;

			if (!Directory.Exists(path)) Globals.Fail(("Cannot find path " + path));

			var allFiles = HydrateDirectory(path, programArgsProxy.Args.Blacklist.ToList());

			var internalFileProxy = Facade.RetrieveProxy(Globals.InternalFileProxyName) as InternalFileProxy;
			if (internalFileProxy == null) Globals.Fail("Could not case Internal File Proxy " + path);

			internalFileProxy.AddFiles(allFiles);

			internalFileProxy.SealProxy();
		}
	}
}
