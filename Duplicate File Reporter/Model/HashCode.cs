using System;

namespace DuplicateFileReporter.Model
{
	public class HashCode : IEquatable<HashCode>
	{
		public HashCode(HashCodeType type, byte[] bytes)
		{
			HashCodeType = type;
			Hash = Convert.ToBase64String(bytes);
		}

		public string Hash { get; private set; }
		public HashCodeType HashCodeType { get; private set; }

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.GetType() == typeof (HashCode) && Equals((HashCode) obj);
		}

		public bool Equals(HashCode other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(other.Hash, Hash) && Equals(other.HashCodeType, HashCodeType);
		}

		public override int GetHashCode()
		{
			return Hash.GetHashCode() ^ HashCodeType.GetHashCode();
		}
	}
}