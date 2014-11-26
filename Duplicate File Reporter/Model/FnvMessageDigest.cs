using System;
using System.Collections.Generic;
using System.IO;

namespace DuplicateFileReporter.Model
{
    /// <summary>
    /// This object is meant to encompass the role of calculating an FNV-a hash code
    /// </summary>
    /// <remarks>
    /// Despite the fact that the _sealed variable is thread-aware, this object is essentially
    /// not thread-safe. It is possible to misuse this object in way that would produce undesirable
    /// or unpredictable results.
    /// 
    /// Algorithm Reference: http://isthe.com/chongo/tech/comp/fnv/#FNV-1a
    /// </remarks>
    public class FnvMessageDigest : IMessageDigest
    {
        private const string MessageDigestName = "FNV1-a Message Digest";

        private const UInt32 ThirtyTwoBitPrime = 16777619;
        private const UInt64 SixtyFourBitPrime = 1099511628211;

        private const UInt32 ThirtyTwoBitOffset = 2166136261;
        private const UInt64 SixtyFourBitOffset = 14695981039346656037;

        private const int BufferSize = 32768;

        //False by default. Needed just in case one thread calls DoFinal while another thread calls Update
        private volatile bool _sealed;

        private UInt32 _32BitHash = ThirtyTwoBitOffset;
        private UInt64 _64BitHash = SixtyFourBitOffset;

        private void UpdateHash(IEnumerable<byte> bytes)
        {
            unchecked
            {
                foreach (var octet in bytes)
                {
                    _32BitHash = (_32BitHash ^ octet) * ThirtyTwoBitPrime;
                    _64BitHash = (_64BitHash ^ octet) * SixtyFourBitPrime;
                }
            }
        }

        /// <summary>
        /// Adds the remaining bytes, finalizes all algorithmic calculations, and seals the object to future
        /// updates.
        /// </summary>
        /// <param name="bytes">The final bytes you want to add to the hash</param>
        /// <exception cref="InvalidOperationException">Throws an exception is the object is already sealed</exception>
        public void DoFinal(IEnumerable<byte> bytes)
        {
            if (_sealed) throw new InvalidOperationException("Cannot call DoFinal on sealed object");

            _sealed = true;
            UpdateHash(bytes);
        }

        /// <summary>
        /// Finalizes any algorithmic calculations and seals the object.
        /// </summary>
        /// <seealso cref="DoFinal()"/>
        public void DoFinal()
        {
            _sealed = true;
        }

        /// <summary>
        /// Updates the hash with the provided bytes
        /// </summary>
        /// <param name="bytes">The bytes you want to update the hash with</param>
        /// <exception cref="InvalidOperationException">Thrown when the object is sealed and cannot be updated</exception>
        public void Update(IEnumerable<byte> bytes)
        {
            if (_sealed) throw new InvalidOperationException("Cannot update a sealed FnvMessageDigest object");

            UpdateHash(bytes);
        }

        /// <summary>
        /// Updates the hash with the provided Stream object
        /// </summary>
        /// <param name="stream">The Stream object you want to use to update the message digest</param>
        /// <exception cref="InvalidOperationException">Thrown when the object is sealed and cannot be updated</exception>
        public void Update(Stream stream)
        {
            if (_sealed) throw new InvalidOperationException("Cannot update a sealed FnvMessageDigest object");

            var buffer = new byte[BufferSize];
            while (stream.Read(buffer, 0, BufferSize) != 0)
            {
                UpdateHash(buffer);
            }
        }

        /// <summary>
        /// Checks to see if the object is sealed or not
        /// </summary>
        /// <returns>bool representing the sealed state of the object</returns>
        public bool IsSealed()
        {
            return _sealed;
        }

        /// <summary>
        /// Resets the object's state—including its hash codes—and unseals the object
        /// </summary>
        public void Reset()
        {
            _32BitHash = ThirtyTwoBitOffset;
            _64BitHash = SixtyFourBitOffset;

            _sealed = false;
        }

        /// <summary>
        /// Gets the 32-bit hash version of this object's hashcode
        /// </summary>
        /// <remarks>The FNVMessageDigest object must be sealed before calling this method</remarks>
        /// <returns>The 32-bit FNV hash</returns>
        public byte[] Get32BitHash()
        {
            if (!_sealed) throw new InvalidOperationException("Cannot get bytes if the FnvMessageDigest isn't sealed");

            return BitConverter.GetBytes(_32BitHash);
        }

        /// <summary>
        /// Gets the 64-bit hash version of this object hashcode
        /// </summary>
        /// <remarks>The FNVMessageDigest object must be sealed before calling this method</remarks>
        /// <returns>The 64-bit FNV hash</returns>
        public byte[] Get64BitHash()
        {
            if (!_sealed) throw new InvalidOperationException("Cannot get bytes if the FnvMessageDigest isn't sealed");

            return BitConverter.GetBytes(_64BitHash);
        }

        public string GetDigestName()
        {
            return MessageDigestName;
        }

        public HashCode GetHash()
        {
            return new HashCode(HashCodeType.Fnv1A32Hash, Get32BitHash());
        }
    }
}
