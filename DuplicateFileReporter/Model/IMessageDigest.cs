using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	public interface IMessageDigest
	{
		string GetDigestName();
		HashCode GetHash();
	}
}
