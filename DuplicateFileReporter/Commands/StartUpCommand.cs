using System.Threading;
using DuplicateFileReporter.Model;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
	public class StartUpCommand
	{
		private static void SendClusterAnalysisNotification()
		{
			Facade.Instance.SendNotification(Globals.ClusterAnalyzeNamesCommand);
		}

		private static void SendHashFilesNotification()
		{
			Facade.Instance.SendNotification(Globals.HashFilesCommand);
		}

		public static void Main(string[] args)
		{
			var facade = Facade.Instance;

			//Register Commands
			facade.RegisterCommand(Globals.ClusterAnalyzeNamesCommand, typeof(ClusterAnalyzeNamesCommand));
			facade.RegisterCommand(Globals.ExitFail, typeof(ExitCommand));
			facade.RegisterCommand(Globals.ExitSuccess, typeof(ExitCommand));
			facade.RegisterCommand(Globals.HashFilesCommand, typeof(HashFilesCommand));
			facade.RegisterCommand(Globals.HydrateInternalFileProxyCommand, typeof(HydrateInternalFileProxyCommand));
			facade.RegisterCommand(Globals.LogErrorNotification, typeof(LogCommand));
			facade.RegisterCommand(Globals.LogInfoNotification, typeof(LogCommand));
			facade.RegisterCommand(Globals.PrintHelpCommand, typeof(PrintHelpCommand));
			facade.RegisterCommand(Globals.ValidateArgsCommand, typeof(ValidateArgsCommand));
			facade.RegisterCommand(Globals.GenerateClusterAnalysisReportCommand, typeof(GenerateClusterAnalysisReportCommand));

			//Register Proxy
			facade.RegisterProxy(new FileHashProxy());
			facade.RegisterProxy(new FileNameClusterProxy());
			facade.RegisterProxy(new InternalFileProxy());
			facade.RegisterProxy(new ProgramArgsProxy(args));
			facade.RegisterProxy(new StringComparisonToolsProxy());
			facade.RegisterProxy(new ThreadPoolProxy());

			//Fire off first notifications
			facade.SendNotification(Globals.LogInfoNotification, "Starting analysis");
			facade.SendNotification(Globals.LogInfoNotification, "Validating Args");
			facade.SendNotification(Globals.ValidateArgsCommand);
			facade.SendNotification(Globals.LogInfoNotification, "Searching for files to analyze");
			facade.SendNotification(Globals.HydrateInternalFileProxyCommand);

			//Fire off string name comparison notification
			var thread1 = new Thread(SendClusterAnalysisNotification);
			var thread2 = new Thread(SendHashFilesNotification);

			//Fire off cluster analysis command
			thread1.Start();
			thread1.Join();

			//Fire off hash analysis
			thread2.Start();
			thread2.Join();

			//Generate Reports
			facade.SendNotification(Globals.LogInfoNotification, "Generating file name clustering report");
			facade.SendNotification(Globals.GenerateClusterAnalysisReportCommand);

			//Shutdown properly
			facade.SendNotification(Globals.LogInfoNotification, "Finished analysis. Shutting down");
			facade.SendNotification(Globals.ExitSuccess);
		}
	}
}
