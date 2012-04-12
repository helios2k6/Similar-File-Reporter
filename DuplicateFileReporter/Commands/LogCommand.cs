﻿
using System;
using System.Globalization;
using System.Text;
using PureMVC.Interfaces;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Commands
{
	public class LogCommand : SimpleCommand
	{
		public void LogError(string msg)
		{
			var builder = new StringBuilder();

			builder.Append("[ERROR] - ")
				.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture))
				.Append(" - ")
				.Append(msg);

			Console.Error.WriteLine(builder.ToString());
		}

		public void LogInfo(string msg)
		{
			var builder = new StringBuilder();

			builder.Append("[INFO] - ")
				.Append(DateTime.Now.ToString(CultureInfo.InvariantCulture))
				.Append(" - ")
				.Append(msg);

			Console.Error.WriteLine(builder.ToString());
		}

		public override void Execute(INotification notification)
		{
			var msg = notification.Body as string;

			if(msg == null)
			{
				LogError("Could not log message");
				return;
			}

			switch(notification.Name)
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
