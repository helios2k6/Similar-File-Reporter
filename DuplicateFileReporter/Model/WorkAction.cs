using System;

namespace DuplicateFileReporter.Model
{
	public class WorkAction
	{
		private readonly Action<object> _action;
		private readonly object _args;

		private volatile bool _invoked;

		public WorkAction(Action<object> action, object args)
		{
			_action = action;
			_args = args;
		}

		public void Invoke()
		{
			_action.Invoke(_args);
			_invoked = true;
		}

		public bool Invoked
		{
			get { return _invoked; }
		}
	}
}
