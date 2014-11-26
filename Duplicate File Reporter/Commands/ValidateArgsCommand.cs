using System.IO;
using System.Linq;
using System.Text;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
    public class ValidateArgsCommand : SimpleCommand
    {
        private static bool IsValidDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public override void Execute(INotification notification)
        {
            var programArgsProxy = Facade.RetrieveProxy(Globals.ProgramArgsProxy) as ProgramArgsProxy;

            if (programArgsProxy == null) Globals.Fail("Could not get ProgramArgsProxy");

            if (programArgsProxy.Args.Help)
            {
                Facade.SendNotification(Globals.PrintHelpCommand);
                Globals.Fail("User asked for help");
            }
            else if (programArgsProxy.InvalidArgs.Any())
            {
                //Log message
                var builder = new StringBuilder();
                builder.Append("Invalid arguments: ");
                foreach (var a in programArgsProxy.InvalidArgs)
                {
                    builder.Append(a).Append(", ");
                }

                builder.AppendLine().AppendLine();

                Facade.SendNotification(Globals.LogErrorNotification, builder.ToString());
                Facade.SendNotification(Globals.PrintHelpCommand);
                Globals.Fail("Could not parse arguments");
            }
            else if (!IsValidDirectory(programArgsProxy.Args.Path))
            {
                Globals.Fail(programArgsProxy.Args.Path + " is not a valid directory");
            }
            else if (!programArgsProxy.Args.UseFnvHash && !programArgsProxy.Args.UseCrc32Hash && !programArgsProxy.Args.UseStringClusterAnalysis)
            {
                Facade.SendNotification(Globals.PrintHelpCommand);
                Globals.Fail("No analysis methods selected");
            }
        }
    }
}
