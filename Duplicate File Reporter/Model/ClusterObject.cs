using System.Collections.Generic;
using System.Text;

namespace DuplicateFileReporter.Model
{
    public sealed class ClusterObject
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

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Cluster ID: " + Id);
            foreach(var file in Files)
            {
                builder.AppendLine(file.ToString());
            }

            return builder.ToString();
        }
    }
}
