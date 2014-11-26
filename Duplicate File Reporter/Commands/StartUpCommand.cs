using System.Collections.Generic;
using System.Threading;
using DuplicateFileReporter.Model;
using PureMVC.Patterns;
using System.Threading.Tasks;

namespace DuplicateFileReporter.Commands
{
    public class StartUpCommand
    {
        private static void SendAnalyzeNamesNotification()
        {
            Facade.Instance.SendNotification(Globals.AnalyzeNamesCommand);
        }

        private static void SendHashFilesNotification()
        {
            Facade.Instance.SendNotification(Globals.HashFilesCommand);
        }

        public static void Main(string[] args)
        {
            var facade = Facade.Instance;

            //Register Commands
            facade.RegisterCommand(Globals.AnalyzeNamesCommand, typeof(AnalyzeNamesCommand));
            facade.RegisterCommand(Globals.ExitFail, typeof(ExitCommand));
            facade.RegisterCommand(Globals.ExitSuccess, typeof(ExitCommand));
            facade.RegisterCommand(Globals.HashFilesCommand, typeof(HashFilesCommand));
            facade.RegisterCommand(Globals.HydrateInternalFileProxyCommand, typeof(HydrateInternalFileProxyCommand));
            facade.RegisterCommand(Globals.LogErrorNotification, typeof(LogCommand));
            facade.RegisterCommand(Globals.LogInfoNotification, typeof(LogCommand));
            facade.RegisterCommand(Globals.PrintHelpCommand, typeof(PrintHelpCommand));
            facade.RegisterCommand(Globals.ValidateArgsCommand, typeof(ValidateArgsCommand));
            facade.RegisterCommand(Globals.GenerateClusterAnalysisReportCommand, typeof(GenerateClusterAnalysisReportCommand));
            facade.RegisterCommand(Globals.GenerateHashReportCommand, typeof(GenerateHashReportCommand));
            facade.RegisterCommand(Globals.OutputReportsCommand, typeof(OutputReportsCommand));

            //Register Proxy
            facade.RegisterProxy(new FileHashProxy());
            facade.RegisterProxy(new FileNameClusterProxy());
            facade.RegisterProxy(new InternalFileProxy());
            facade.RegisterProxy(new ProgramArgsProxy(args));
            facade.RegisterProxy(new StringComparisonToolsProxy());
            facade.RegisterProxy(new ReportProxy());

            //Fire off first notifications
            facade.SendNotification(Globals.LogInfoNotification, "Starting analysis");
            facade.SendNotification(Globals.LogInfoNotification, "Validating Args");
            facade.SendNotification(Globals.ValidateArgsCommand);
            facade.SendNotification(Globals.LogInfoNotification, "Searching for files to analyze");
            facade.SendNotification(Globals.HydrateInternalFileProxyCommand);

            var programArgs = ((ProgramArgsProxy)facade.RetrieveProxy(Globals.ProgramArgsProxy)).Args;
            var listOfTasks = new List<Task>();

            if (programArgs.UseCrc32Hash || programArgs.UseFnvHash)
            {
                listOfTasks.Add(Task.Factory.StartNew(SendHashFilesNotification));
            }

            if (programArgs.UseStringClusterAnalysis)
            {
                listOfTasks.Add(Task.Factory.StartNew(SendAnalyzeNamesNotification));
            }

            foreach (var t in listOfTasks)
            {
                t.Wait();
            }

            //Generate reports
            facade.SendNotification(Globals.GenerateClusterAnalysisReportCommand);
            facade.SendNotification(Globals.GenerateHashReportCommand);

            //Print reports out
            facade.SendNotification(Globals.OutputReportsCommand);

            //Shutdown properly
            facade.SendNotification(Globals.LogInfoNotification, "Finished analysis. Shutting down");
            facade.SendNotification(Globals.ExitSuccess);
        }
    }
}
