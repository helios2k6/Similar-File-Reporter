using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool = DuplicateFileReporter.Model.ThreadPool;

namespace UnitTests
{
	[TestClass]
	public class ThreadPoolTest
	{
		private static void WorkMethod(object args)
		{
			Thread.Sleep(5000);
			Console.WriteLine("Work done");
		}

		[TestMethod]
		public void SubmitActionTest()
		{
			var pool = new ThreadPool(8);

			for(var i = 0; i < 40; i++)
			{
				pool.SubmitAction(WorkMethod, new object());
			}

			pool.Shutdown();

			pool.WaitOnAllWork();
		}

		[TestMethod]
		public void TestZeroLengthWork()
		{
			var pool = new ThreadPool(8);

			pool.Shutdown();

			pool.WaitOnAllWork();
		}

		[TestMethod]
		public void TestNull()
		{
			var pool = new ThreadPool(8);

			pool.SubmitAction(WorkMethod, null);

			pool.Shutdown();

			pool.WaitOnAllWork();
		}
	}
}
