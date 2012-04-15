using System;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
	public class ExitCommand : SimpleCommand
	{
		private void CleanUp()
		{
			var threadPoolProxy = Facade.RetrieveProxy(Globals.ThreadPoolProxy) as ThreadPoolProxy;
			if (threadPoolProxy == null) return;

			threadPoolProxy.Threadpool.Shutdown();
			threadPoolProxy.Threadpool.WaitOnAllWork();
		}

		private void ExitFail()
		{
			CleanUp();
			Facade.SendNotification(Globals.LogErrorNotification, "Exit Failure");
			Environment.Exit(1);
		}

		private void ExitSuccess()
		{
			CleanUp();
			Facade.SendNotification(Globals.LogInfoNotification, "Exit Success");
			Environment.Exit(0);
		}

		public override void Execute(INotification notification)
		{
			var exitType = notification.Name;

			switch(exitType)
			{
				case Globals.ExitFail:
					ExitFail();
					break;
				case Globals.ExitSuccess:
					ExitSuccess();
					break;
			}
		}
	}
}
