﻿using System.Collections.Generic;

namespace DuplicateFileReporter.Model
{
	using System;
	using System.Security.Cryptography;

	public class Crc32MessageDigest : HashAlgorithm, IMessageDigest
	{
		public const string Crc32MessageDigestHash = "CRC-32 Message Digest Hash";

		public const UInt32 DefaultPolynomial = 0xedb88320;
		public const UInt32 DefaultSeed = 0xffffffff;

		private UInt32 _hash;
		private readonly UInt32 _seed;
		private readonly UInt32[] _table;
		private static UInt32[] _defaultTable;

		public Crc32MessageDigest()
		{
			_table = InitializeTable(DefaultPolynomial);
			_seed = DefaultSeed;
			Initialize();
		}

		public Crc32MessageDigest(UInt32 polynomial, UInt32 seed)
		{
			_table = InitializeTable(polynomial);
			_seed = seed;
			Initialize();
		}

		public override sealed void Initialize()
		{
			_hash = _seed;
		}

		protected override void HashCore(byte[] buffer, int start, int length)
		{
			_hash = CalculateHash(_table, _hash, buffer, start, length);
		}

		protected override byte[] HashFinal()
		{
			var hashBuffer = UInt32ToBigEndianBytes(~_hash);
			HashValue = hashBuffer;
			return hashBuffer;
		}

		public override int HashSize
		{
			get { return 32; }
		}

		public static UInt32 Compute(byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(DefaultPolynomial), DefaultSeed, buffer, 0, buffer.Length);
		}

		public static UInt32 Compute(UInt32 seed, byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(DefaultPolynomial), seed, buffer, 0, buffer.Length);
		}

		public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
		{
			return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
		}

		private static UInt32[] InitializeTable(UInt32 polynomial)
		{
			if (polynomial == DefaultPolynomial && _defaultTable != null)
				return _defaultTable;

			var createTable = new UInt32[256];
			for (var i = 0; i < 256; i++)
			{
				var entry = (UInt32)i;
				for (var j = 0; j < 8; j++)
					if ((entry & 1) == 1)
						entry = (entry >> 1) ^ polynomial;
					else
						entry = entry >> 1;
				createTable[i] = entry;
			}

			if (polynomial == DefaultPolynomial)
				_defaultTable = createTable;

			return createTable;
		}

		private static UInt32 CalculateHash(UInt32[] table, UInt32 seed, IList<byte> buffer, int start, int size)
		{
			var crc = seed;
			for (var i = start; i < size; i++)
				unchecked
				{
					crc = (crc >> 8) ^ table[buffer[i] ^ crc & 0xff];
				}
			return crc;
		}

		private static byte[] UInt32ToBigEndianBytes(UInt32 x)
		{
			return new[] {
			(byte)((x >> 24) & 0xff),
			(byte)((x >> 16) & 0xff),
			(byte)((x >> 8) & 0xff),
			(byte)(x & 0xff)
		};
		}

		public string GetDigestName()
		{
			return Crc32MessageDigestHash;
		}
	}
}
