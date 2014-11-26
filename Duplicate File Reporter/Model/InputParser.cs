using System.Collections.Generic;
using System.Linq;

namespace DuplicateFileReporter.Model
{
    public sealed class InputParser
    {
        private readonly IDictionary<string, IList<string>> _argCollector = new Dictionary<string, IList<string>>();

        private readonly IList<string> _invalidArgs = new List<string>();

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

            foreach (var a in args)
            {
                if (validArgs.Contains(a))
                {
                    currentSwitch = a;
                    if (!_argCollector.ContainsKey(a))
                    {
                        _argCollector[a] = new List<string>();
                    }
                }
                else if (a.Substring(0, 2).Equals(ProgramArgsConstants.ValidArgPrefix))
                {
                    _invalidArgs.Add(a);
                }
                else
                {
                    _argCollector[currentSwitch].Add(a);
                }
            }
        }

        public bool FoundInvalidArgs
        {
            get { return _invalidArgs.Any(); }
        }

        public IEnumerable<string> InvalidArgs
        {
            get { return _invalidArgs; }
        }
    }
}
