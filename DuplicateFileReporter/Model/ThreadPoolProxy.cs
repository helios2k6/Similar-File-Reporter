using System;
using PureMVC.Patterns;

namespace DuplicateFileReporter.Model
{
	public class ThreadPoolProxy : Proxy
	{
		public ThreadPoolProxy() : base(Globals.ThreadPoolProxy)
		{
			Threadpool = new ThreadPool(Environment.ProcessorCount);
		}

		public ThreadPool Threadpool { get; private set; }
	}
}
