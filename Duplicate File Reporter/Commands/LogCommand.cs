
using System;
using System.Globalization;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
    public sealed class LogCommand : SimpleCommand
    {
        private readonly object _lockObject = new object();

        private void LogError(string msg)
        {
            var builder = new StringBuilder();

            builder.Append("[ERROR] - ")
                .Append(DateTime.Now.ToString(CultureInfo.InvariantCulture))
                .Append(" - ")
                .Append(msg);
            lock (_lockObject)
            {
                Console.Error.WriteLine(builder.ToString());
            }
        }

        private void LogInfo(string msg)
        {
            var builder = new StringBuilder();

            builder.Append("[INFO] - ")
                .Append(DateTime.Now.ToString(CultureInfo.InvariantCulture))
                .Append(" - ")
                .Append(msg);

            lock (_lockObject)
            {
                Console.Error.WriteLine(builder.ToString());
            }
        }

        public override void Execute(INotification notification)
        {
            var msg = notification.Body as string;

            if (msg == null)
            {
                LogError("Could not log message");
                return;
            }

            switch (notification.Name)
            {
                case Globals.LogInfoNotification:
                    LogInfo(msg);
                    break;
                case Globals.LogErrorNotification:
                    LogError(msg);
                    break;
            }
        }
    }
}
