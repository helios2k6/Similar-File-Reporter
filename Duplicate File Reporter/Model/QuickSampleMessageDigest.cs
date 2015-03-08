using System;
using System.IO;
using System.Linq;

namespace DuplicateFileReporter.Model
{
    public sealed class QuickSampleMessageDigest : IMessageDigest
    {
        private readonly InternalFile _file;
        private readonly Lazy<HashCode> _lazyHashCode;
        private readonly Lazy<long> _fileLength;

        public InternalFile File { get { return _file; } }
        public long FileLength { get { return _fileLength.Value; } }

        public QuickSampleMessageDigest(InternalFile file)
        {
            _file = file;
            _lazyHashCode = new Lazy<HashCode>(CalculateHashCode);
            _fileLength = new Lazy<long>(CalculateFileLength);
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
            var sampleHashArray = BitConverter.GetBytes(SampleFile(_file.FilePath, FileLength));
            var lengthArray = BitConverter.GetBytes(FileLength);

            return new HashCode(HashCodeType.SampleHash, sampleHashArray.Concat(lengthArray).ToArray());
        }

        private long CalculateFileLength()
        {
            return new FileInfo(_file.FilePath).Length;
        }

        private static long SampleFile(string filePath, long fileLength)
        {
            long sampleHash = 0;
            byte[] buffer = new byte[8]; //Size of Int64
            if (fileLength > 0)
            {
                using (var stream = System.IO.File.OpenRead(filePath))
                {
                    //First 4 bytes
                    stream.Read(buffer, 0, 4);

                    //Last 4 bytes (remember, might be the same as the first 4 bytes)
                    if (fileLength > 4)
                    {
                        //Go 4 bytes back from the end of the file
                        stream.Seek(-4, SeekOrigin.End);
                        stream.Read(buffer, 4, 4);
                    }
                }

                sampleHash = BitConverter.ToInt64(buffer, 0);
            }

            return sampleHash;
        }
    }
}
