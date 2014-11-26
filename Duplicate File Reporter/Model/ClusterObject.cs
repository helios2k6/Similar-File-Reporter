using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
    public class ClusterObject
    {
        private static volatile int _idIncrementer;

        private readonly ICollection<InternalFile> _files = new HashSet<InternalFile>();

        public ClusterObject()
        {
            Id = _idIncrementer++;
        }

        public int Id { get; set; }

        public void AddFile(InternalFile file)
        {
            lock (_files)
            {
                _files.Add(file);
            }
        }

        public IEnumerable<InternalFile> Files
        {
            get { return _files; }
        }

        public int Count { get { return _files.Count; } }
    }
}
