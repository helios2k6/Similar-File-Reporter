using System;
using System.Text;
using DuplicateFileReporter.Model;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
    public class PrintHelpCommand : SimpleCommand
    {
        private static string GetPrologue()
        {
            return "\nDuplicate File Reporter 2012.1.0\n\nAuthor: Andrew Johnson\n\nUsage <this program> [options]";
        }

        public override void Execute(INotification notification)
        {
            var builder = new StringBuilder();
            builder.Append(GetPrologue()).AppendLine().AppendLine().Append("Options:").AppendLine();
            foreach (var s in ProgramArgsConstants.ProgramArgsHelpMap)
            {
                builder.Append("\t").Append(s.Key).AppendLine().Append("\t\t\t\t").Append(s.Value).AppendLine().AppendLine();
            }

            Console.Write(builder.ToString());
        }
    }
}
