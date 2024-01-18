﻿#region License
// Copyright (c) 2014, David Taylor
//
// Permission to use, copy, modify, and/or distribute this software for any 
// purpose with or without fee is hereby granted, provided that the above 
// copyright notice and this permission notice appear in all copies, unless 
// such copies are solely in the form of machine-executable object code 
// generated by a source language processor.
//
// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES 
// WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF 
// MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR 
// ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES 
// WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN 
// ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF 
// OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
#endregion License
using System;
#if NET5_0_OR_GREATER
using System.Buffers.Binary;
#endif
using System.IO;
using System.Text;

namespace HotChai.Serialization.PortableBinary
{
    /// <summary>
    /// Reads an object using the Portable Binary Object Notation format.
    /// </summary>
    public sealed class PortableBinaryObjectReader : ObjectReader
    {
        private readonly InspectorStream _stream;
        private readonly BinaryReader _reader;
        private bool _peeked;
        private int _peekedValue;
        private byte[] _skipBuffer;

        /// <summary>
        /// Initializes a new instance of the <c>PortableBinaryObjectReader</c> 
        /// using the specified input stream.
        /// </summary>
        /// <param name="stream">The input stream.</param>
        public PortableBinaryObjectReader(
            Stream stream)
        {
            if (null == stream)
            {
                throw new ArgumentNullException("stream");
            }

            this._stream = new InspectorStream(stream);
            this._reader = new BinaryReader(this._stream);
        }

        public override ISerializationInspector Inspector
        {
            get
            {
                return this._stream.Inspector;
            }

            set
            {
                this._stream.Inspector = value;
            }
        }

        /// <summary>
        /// Reads the start of a serialized object at the current position.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the start of the serialized object was read, or <c>false</c> if 
        /// a null object value was read.
        /// </returns>
        protected override bool ReadStartObjectToken()
        {
            int encodedToken = ReadPackedInt();
            if (encodedToken == PortableBinaryToken.StartObjectToken)
            {
                return true;
            }
            else if (encodedToken == PortableBinaryToken.NullValueToken)
            {
                return false;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Reads the next serialized object member key at the current position.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the member key was read, or <c>false</c> if the 
        /// end of the seralized object was encountered.
        /// </returns>
        protected override bool ReadNextObjectMemberKey()
        {
            // Read the next packed int
            int packedInt = ReadPackedInt();

            // Check for an end object token
            if (packedInt == PortableBinaryToken.EndObjectToken)
            {
                return false;
            }

            // Check for an unexpected key value
            // NOTE: Negative values are reserved for tokens
            if (packedInt <= 0)
            {
                throw new InvalidOperationException();
            }

            // Set the current object member key
            this.MemberKey = packedInt;

            return true;
        }

        /// <summary>
        /// Reads the end of the serialized object at the current position.
        /// </summary>
        protected override void ReadEndObjectToken()
        {
            // Already read the end object token, 
            // so nothing to do here for this encoding.
        }

        /// <summary>
        /// Reads the start of a serialized array at the current position.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the start of the array was read, or <c>false</c> if 
        /// a null array value was read.
        /// </returns>
        protected override bool ReadStartArrayToken()
        {
            int encodedToken = ReadPackedInt();
            if (encodedToken == PortableBinaryToken.StartArrayToken)
            {
                return true;
            }
            else if (encodedToken == PortableBinaryToken.NullValueToken)
            {
                return false;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Advances the reader to the next array value following the current position.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the reader was advanced, or <c>false</c> if 
        /// the end of the array was encountered.
        /// </returns>
        protected override bool ReadToNextArrayValue()
        {
            int packedInt = PeekPackedInt();
            if (packedInt == PortableBinaryToken.EndArrayToken)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reads the end of the serialized array at the current position.
        /// </summary>
        protected override void ReadEndArrayToken()
        {
            int encodedToken = ReadPackedInt();
            if (encodedToken != PortableBinaryToken.EndArrayToken)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Reads the current value as a <c>Boolean</c> type.
        /// </summary>
        /// <returns>The <c>Boolean</c> value.</returns>
        protected override bool ReadPrimitiveValueAsBoolean()
        {
            int encodedToken = ReadPackedInt();
            if (encodedToken == PortableBinaryToken.TrueValueToken)
            {
                return true;
            }
            else if (encodedToken == PortableBinaryToken.FalseValueToken)
            {
                return false;
            }
            else
            {
                throw new InvalidOperationException("Unexpected Boolean value.");
            }
        }

        /// <summary>
        /// Reads the current value as an <c>Int32</c> type.
        /// </summary>
        /// <returns>The <c>Int32</c> value.</returns>
        protected override int ReadPrimitiveValueAsInt32()
        {
            Int32 value;

            int length = ReadPrimitiveLength(4);

            // First byte contains sign flag
            bool negative = false;
            value = this._reader.ReadByte();
            if (0x80 == (value & 0x80))
            {
                negative = true;
                value = value & 0x7f;
            }

            // Read remainder of bytes in base 256
            for (int i = 1; i < length; ++i)
            {
                value = (value * 256) + this._reader.ReadByte();
            }

            if (negative)
            {
                value = ~value;
            }

            return value;
        }

        /// <summary>
        /// Reads the current value as a <c>UInt32</c> type.
        /// </summary>
        /// <returns>The <c>UInt32</c> value.</returns>
        protected override uint ReadPrimitiveValueAsUInt32()
        {
            // Read base 256 encoded value
            UInt32 value = 0;
            int length = ReadPrimitiveLength(4);
            for (int i = 0; i < length; ++i)
            {
                value = (value * 256) + this._reader.ReadByte();
            }

            return value;
        }

        /// <summary>
        /// Reads the current value as an <c>Int64</c> type.
        /// </summary>
        /// <returns>The <c>Int64</c> value.</returns>
        protected override long ReadPrimitiveValueAsInt64()
        {
            Int64 value;

            int length = ReadPrimitiveLength(8);

            // First byte contains sign flag
            bool negative = false;
            value = this._reader.ReadByte();
            if (0x80 == (value & 0x80))
            {
                negative = true;
                value = value & 0x7f;
            }

            // Read remainder of bytes in base 256
            for (int i = 1; i < length; ++i)
            {
                value = (value * 256) + this._reader.ReadByte();
            }

            if (negative)
            {
                value = ~value;
            }

            return value;
        }

        /// <summary>
        /// Reads the current value as a <c>UInt64</c> type.
        /// </summary>
        /// <returns>The <c>UInt64</c> value.</returns>
        protected override ulong ReadPrimitiveValueAsUInt64()
        {
            // Read base 256 encoded value
            UInt64 value = 0;
            int length = ReadPrimitiveLength(8);
            for (int i = 0; i < length; ++i)
            {
                value = (value * 256) + this._reader.ReadByte();
            }

            return value;
        }

#if NET5_0_OR_GREATER
        /// <summary>
        /// Reads the current value as a <c>Single</c> type.
        /// </summary>
        /// <returns>The <c>Single</c> value.</returns>
        protected override float ReadPrimitiveValueAsSingle()
        {
            int length = ReadPrimitiveLength(4);
            Span<byte> buffer = stackalloc byte[length];
            this._reader.Read(buffer);
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
        }

        /// <summary>
        /// Reads the current value as a <c>Double</c> type.
        /// </summary>
        /// <returns>The <c>Double</c> value.</returns>
        protected override double ReadPrimitiveValueAsDouble()
        {
            int length = ReadPrimitiveLength(8);
            Span<byte> buffer = stackalloc byte[length];
            this._reader.Read(buffer);
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
        }
#else
        /// <summary>
        /// Reads the current value as a <c>Single</c> type.
        /// </summary>
        /// <returns>The <c>Single</c> value.</returns>
        protected override float ReadPrimitiveValueAsSingle()
        {
            int length = ReadPrimitiveLength(4);
            byte[] bytes = this._reader.ReadBytes(length);
            if (BitConverter.IsLittleEndian)
            {
                // Convert from network order (big-endian)
                Array.Reverse(bytes);
            }
            return BitConverter.ToSingle(bytes, 0);
        }

        /// <summary>
        /// Reads the current value as a <c>Double</c> type.
        /// </summary>
        /// <returns>The <c>Double</c> value.</returns>
        protected override double ReadPrimitiveValueAsDouble()
        {
            int length = ReadPrimitiveLength(8);
            byte[] bytes = this._reader.ReadBytes(length);
            if (BitConverter.IsLittleEndian)
            {
                // Convert from network order (big-endian)
                Array.Reverse(bytes);
            }
            return BitConverter.ToDouble(bytes, 0);
        }
#endif

        /// <summary>
        /// Reads the current value as an array of <c>Byte</c> type.
        /// </summary>
        /// <returns>The array of <c>Byte</c> value.</returns>
        protected override byte[] ReadPrimitiveValueAsBytes(
            int quota)
        {
            int length = ReadNullablePrimitiveLength(quota);
            if (length < 0)
            {
                return null;
            }

            return this._reader.ReadBytes(length);
        }

        /// <summary>
        /// Reads the current value as a <c>String</c> type.
        /// </summary>
        /// <returns>The <c>String</c> value.</returns>
        protected override string ReadPrimitiveValueAsString(
            int quota)
        {
            int length = ReadNullablePrimitiveLength(quota);
            if (length < 0)
            {
                return null;
            }

            byte[] bytes = this._reader.ReadBytes(length);
            return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Returns the <c>MemberType</c> of the value at the current 
        /// reader position, without advancing the reader.
        /// </summary>
        /// <returns>The <c>MemberType</c>.</returns>
        protected override MemberValueType PeekValueType()
        {
            MemberValueType memberValueType;

            int peekedInt = PeekPackedInt();

            if (peekedInt < 0)
            {
                if (peekedInt == PortableBinaryToken.StartObjectToken)
                {
                    memberValueType = MemberValueType.Object;
                }
                else if (peekedInt == PortableBinaryToken.StartArrayToken)
                {
                    memberValueType = MemberValueType.Array;
                }
                else if (peekedInt == PortableBinaryToken.NullValueToken)
                {
                    // Treat a null token as a primitive value
                    memberValueType = MemberValueType.Primitive;
                }
                else if ((peekedInt == PortableBinaryToken.TrueValueToken)
                    || (peekedInt == PortableBinaryToken.FalseValueToken))
                {
                    // Treat Boolean tokens as a primitive value
                    memberValueType = MemberValueType.Primitive;
                }
                else
                {
                    throw new NotSupportedException("Unexpected token.");
                }
            }
            else
            {
                memberValueType = MemberValueType.Primitive;
            }

            return memberValueType;
        }

        /// <summary>
        /// Skips the primitive value at the current reader position.
        /// </summary>
        protected override void SkipPrimitiveValue()
        {
            int length = ReadNullablePrimitiveLength(Int32.MaxValue);
            if (length <= 0)
            {
                return;
            }

            if (!TrySkipBytes(length))
            {
                throw new InvalidOperationException();
            }
        }

        private bool TrySkipBytes(
            int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", "Count must be a non-negative integer.");
            }

            if (this._stream.CanSeek)
            {
                this._stream.Seek(count, SeekOrigin.Current);
            }
            else
            {
                if (this._skipBuffer is null)
                {
                    // TODO: Pooled buffer
                    this._skipBuffer = new byte[4096];
                }

                int bytesRead;

                while (count > 0)
                {
                    bytesRead = this._stream.Read(this._skipBuffer, 0, count > this._skipBuffer.Length ? this._skipBuffer.Length : count);
                    if (0 == bytesRead)
                    {
                        // End of stream
                        return false;
                    }

                    count -= bytesRead;
                }
            }

            return true;
        }

        private int ReadPrimitiveLength(
            int quota)
        {
            int length = ReadPackedInt();

            if (length < 0)
            {
                throw new InvalidOperationException("Read an invalid length.");
            }
            else if (length > quota)
            {
                throw new InvalidOperationException("Exceeded quota.");
            }

            return length;
        }

        private int ReadNullablePrimitiveLength(
            int quota)
        {
            int length = ReadPackedInt();

            if (length == PortableBinaryToken.NullValueToken)
            {
                length = -1;
            }
            else if ((length == PortableBinaryToken.TrueValueToken)
                || (length == PortableBinaryToken.FalseValueToken))
            {
                length = 0;
            }
            else if (length < 0)
            {
                throw new InvalidOperationException("Read an invalid length.");
            }
            else if (length > quota)
            {
                throw new InvalidOperationException("Exceeded quota.");
            }

            return length;
        }

        private int PeekPackedInt()
        {
            if (!this._peeked)
            {
                this._peekedValue = ReadPackedInt();
                this._peeked = true;
            }

            return this._peekedValue;
        }

        /// <summary>
        /// Big-endian variable-length quantity (VLQ) encoding
        /// </summary>
        /// <remarks>
        /// See http://en.wikipedia.org/wiki/Variable-length_quantity.
        /// </remarks>
        private int ReadPackedInt()
        {
            if (this._peeked)
            {
                this._peeked = false;

                return this._peekedValue;
            }

            // Read the first byte
            int readByte = this._reader.ReadByte();
            if (readByte == -1)
            {
                throw new InvalidOperationException();
            }

            byte byteValue = (byte)readByte;

            // Bit 6 contains the sign
            bool isNegative = ((byteValue & 0x40) != 0);

            // Bits 0-5 contain the most significant 6 bits of the value
            int value = byteValue & 0x3f;

            // Remainder of value is the base 128 encoded absolute value (big-endian),
            // with the MSB set as a continuation bit.

            // Check the continuation bit (bit 7)
            while ((byteValue & 0x80) != 0)
            {
                // Read the next byte
                readByte = this._reader.ReadByte();
                if (readByte == -1)
                {
                    throw new InvalidOperationException();
                }

                byteValue = (byte)readByte;

                // Bits 0-6 contain the next 7 least significant bits of the value
                value = (value << 7) | (byteValue & 0x7f);
            }

            if (isNegative)
            {
                value = ~value;
            }

            return value;
        }
    }
}
