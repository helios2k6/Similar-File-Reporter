
using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using DuplicateFileReporter.Model;
using Newtonsoft.Json;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
    public sealed class OutputReportsCommand : SimpleCommand
    {
        private string OutputCommandsToFlat()
        {
            var reportProxy = Facade.RetrieveProxy<ReportProxy>(Globals.ReportProxy);
            var builder = new StringBuilder();

            foreach (var r in reportProxy.Reports)
            {
                builder.AppendLine(r.ToString());
            }

            return builder.ToString();
        }

        private string OutputCommandsToJson()
        {
            var reportProxy = Facade.RetrieveProxy<ReportProxy>(Globals.ReportProxy);
            var builder = new StringBuilder();

            foreach (var r in reportProxy.Reports)
            {
                builder.Append(JsonConvert.SerializeObject(r, Formatting.Indented)).AppendLine().AppendLine();
            }

            return builder.ToString();
        }

        private string OutputCommandsToXml()
        {
            var reportProxy = Facade.RetrieveProxy<ReportProxy>(Globals.ReportProxy);
            var builder = new StringBuilder();
            var stringWriter = new StringWriter(builder);
            var serializer = new XmlSerializer(typeof(Report));

            foreach (var r in reportProxy.Reports)
            {
                serializer.Serialize(stringWriter, r);

                builder.AppendLine().AppendLine();
            }

            stringWriter.Close();

            return builder.ToString();
        }

        public override void Execute(INotification notification)
        {
            var programArgsProxy = Facade.RetrieveProxy<ProgramArgsProxy>(Globals.ProgramArgsProxy);
            var typeOfCommand = programArgsProxy.Args.OutputFileFormat;
            var serializationResult = string.Empty;

            switch (typeOfCommand)
            {
                case OutputReportType.FLAT:
                    serializationResult = OutputCommandsToFlat();
                    break;
                case OutputReportType.JSON:
                    serializationResult = OutputCommandsToJson();
                    break;
                case OutputReportType.XML:
                    serializationResult = OutputCommandsToXml();
                    break;
            }

            switch (programArgsProxy.Args.OutputFile)
            {
                case "":
                    Console.WriteLine(serializationResult);
                    break;
                default:
                    using (var outFile = new StreamWriter(programArgsProxy.Args.OutputFile))
                    {
                        outFile.Write(serializationResult);
                    }
                    break;
            }
        }
    }
}
