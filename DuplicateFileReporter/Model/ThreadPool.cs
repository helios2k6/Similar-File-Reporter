﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace DuplicateFileReporter.Model
{
	public class ThreadPool
	{
		private readonly ConcurrentQueue<WorkAction> _workQueue = new ConcurrentQueue<WorkAction>();

		private readonly ConcurrentDictionary<WorkAction, ManualResetEvent> _notifications = new ConcurrentDictionary<WorkAction, ManualResetEvent>();

		private readonly IList<Thread> _threads = new List<Thread>();

		private volatile bool _isShutdown;

		private readonly Semaphore _workItems = new Semaphore(0, int.MaxValue);

		private readonly int _maxThreads;

		public ThreadPool(int maxThreads)
		{
			for(var i = 0; i < maxThreads; i++)
			{
				var t = new Thread(DoWorkLoop);

				_threads.Add(t);

				t.Start();
			}

			_maxThreads = maxThreads;
		}

		/// <summary>
		/// Main loop that each thread will go through
		/// </summary>
		private void DoWorkLoop()
		{
			while(!_isShutdown || _workQueue.Count > 0)
			{
				while (!_workItems.WaitOne())
				{
				}
				WorkAction action;
				if(_workQueue.TryDequeue(out action))
				{
					action.Invoke();
					_notifications[action].Set();
				}
			}
		}

		public ManualResetEvent SubmitAction(Action<object> action, object args)
		{
			if(_isShutdown) throw new InvalidOperationException("Cannot submit work to threadpool when it is shutdown");

			var workAction = new WorkAction(action, args);
			var notification = new ManualResetEvent(false);
			_workQueue.Enqueue(workAction);

			if (!_notifications.TryAdd(workAction, notification))
				throw new Exception("Could not map WorkAction to Notification (AutoResetEvent)");

			_workItems.Release();

			return notification;
		}

		public void Shutdown()
		{
			_isShutdown = true;
			_workItems.Release(_maxThreads);
		}

		public bool IsShutdown
		{
			get { return _isShutdown; }
		}

		public void WaitOnAllWork()
		{
			foreach(var t in _threads)
			{
				t.Join();
			}
		}
	}
}