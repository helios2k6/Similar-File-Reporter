using System;
using System.IO;
using System.Linq;

namespace DuplicateFileReporter.Model
{
    public sealed class QuickSampleMessageDigest : IMessageDigest
    {
        private sealed class SampledFile
        {
            public long SampleHash { get; set; }
            public long FileSize { get; set; }
        }

        private readonly InternalFile _file;
        private readonly Lazy<HashCode> _lazyHashCode;

        public InternalFile File { get { return _file; } }

        public QuickSampleMessageDigest(InternalFile file)
        {
            _file = file;
            _lazyHashCode = new Lazy<HashCode>(CalculateHashCode);
        }

        public string GetDigestName()
        {
            return "Quick Sample Message Digest";
        }

        public HashCode GetHash()
        {
            return _lazyHashCode.Value;
        }

        private HashCode CalculateHashCode()
        {
            var sampledFile = SampleFile(_file);
            var sampleHashArray = BitConverter.GetBytes(sampledFile.SampleHash);
            var lengthArray = BitConverter.GetBytes(sampledFile.FileSize);

            return new HashCode(HashCodeType.SampleHash, sampleHashArray.Concat(lengthArray).ToArray());
        }

        private static SampledFile SampleFile(InternalFile file)
        {
            long sampleHash = 0;
            byte[] buffer = new byte[8]; //Size of Int64
            var fileInfo = new FileInfo(file.FilePath);

            if (fileInfo.Length > 0)
            {
                using (var stream = System.IO.File.OpenRead(file.FilePath))
                {
                    //First 4 bytes
                    stream.Read(buffer, 0, 4);

                    //Last 4 bytes (remember, might be the same as the first 4 bytes)
                    if (fileInfo.Length > 4)
                    {
                        //Go 4 bytes back from the end of the file
                        stream.Seek(-4, SeekOrigin.End);
                        stream.Read(buffer, 4, 4);
                    }
                }

                sampleHash = BitConverter.ToInt64(buffer, 0);
            }

            return new SampledFile
            {
                SampleHash = sampleHash,
                FileSize = fileInfo.Length,
            };
        }
    }
}
