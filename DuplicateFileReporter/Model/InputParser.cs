using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	public class InputParser
	{
		private readonly IDictionary<string, IList<string>> _argCollector = new Dictionary<string, IList<string>>();

		public InputParser(IEnumerable<string> args)
		{
			Initialize(args);
		}

		public IDictionary<string, IList<string>> ArgCollection
		{
			get { return _argCollector; }
		}

		private void Initialize(IEnumerable<string> args)
		{
			var validArgs = ProgramArgsConstants.ValidProgramArgsVector;
			var currentSwitch = string.Empty;
			var invalidArgs = new HashSet<string>();

			foreach(var a in args)
			{
				if(validArgs.Contains(a))
				{
					currentSwitch = a;
					if(!_argCollector.ContainsKey(a))
					{
						_argCollector[a] = new List<string>();
					}
				}else if(a.Substring(0, 2).Equals(ProgramArgsConstants.ValidArgPrefix))
				{
					invalidArgs.Add(a);
				}else
				{
					_argCollector[currentSwitch].Add(a);
				}
			}
		}
	}
}
