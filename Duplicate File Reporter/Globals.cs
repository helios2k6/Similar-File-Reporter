using PureMVC.Patterns;

namespace DuplicateFileReporter
{
    public static class Globals
    {
        //Commands
        //Exit Command
        public const string ExitSuccess = "ExitSuccess";
        public const string ExitFail = "ExitFail";

        //Core Function Commands
        public const string HydrateInternalFileProxyCommand = "HydrateInternalFileProxyCommand";
        public const string AnalyzeNamesCommand = "ClusterAnalyzeNamesCommand";
        public const string HashFilesCommand = "HashFilesCommand";
        public const string PrintHelpCommand = "PrintHelpCommand";
        public const string StartUpCommand = "StartUpCommand";
        public const string ValidateArgsCommand = "ValidateArgsCommand";
        public const string GenerateClusterAnalysisReportCommand = "GenerateClusterAnalysisReportCommand";
        public const string GenerateHashReportCommand = "GenerateHashReportCommand";
        public const string OutputReportsCommand = "OutputReportsCommand";

        //Log Command
        public const string LogInfoNotification = "LogInfo";
        public const string LogErrorNotification = "LogError";

        //Proxy Names
        public const string InternalFileProxyName = "InternalFileProxyName";
        public const string FileNameClusterProxy = "FileNameClusterProxy";
        public const string FileHashProxy = "FileHashProxy";
        public const string ProgramArgsProxy = "ProgramArgsProxy";
        public const string ThreadPoolProxy = "ThreadPoolProxy";
        public const string StringComparisonToolsProxy = "StringComparisonToolsProxy";
        public const string ReportProxy = "ReportProxy";

        //Convienent Fail Command
        public static void Fail(string msg)
        {
            Facade.Instance.SendNotification(LogErrorNotification, msg);
            Facade.Instance.SendNotification(ExitFail);
        }
    }
}
