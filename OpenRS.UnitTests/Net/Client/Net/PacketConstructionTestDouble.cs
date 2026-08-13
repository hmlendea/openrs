using System;
using System.Collections.Generic;
using System.IO;

using OpenRS.Net.Client.Net;

namespace OpenRS.UnitTests.Net.Client.Net
{
    internal sealed class PacketConstructionTestDouble : PacketConstruction
    {
        private readonly Queue<int> readValues = [];
        private readonly List<byte> writtenBytes = [];

        internal IEnumerable<byte> WrittenBytes => writtenBytes;

        internal bool DoesReadThrow { get; set; }

        internal bool DoesWriteThrow { get; set; }

        internal bool IsCloseStreamInvoked { get; private set; }

        internal int LastReadSize { get; private set; }

        internal int LastReadType { get; private set; }

        internal int ReadInputStreamInvocationCount { get; private set; }

        internal int WriteToBufferInvocationCount { get; private set; }

        public override void CloseStream() => IsCloseStreamInvoked = true;

        public override int Read()
        {
            if (DoesReadThrow)
            {
                throw new IOException("The test input stream failed.");
            }

            return readValues.Dequeue();
        }

        public override void ReadInputStream(int size, int type, sbyte[] buffer)
        {
            LastReadSize = size;
            LastReadType = type;
            ReadInputStreamInvocationCount += 1;

            for (int byteIndex = 0; byteIndex < size; byteIndex += 1)
            {
                buffer[byteIndex] = (sbyte)Read();
            }
        }

        public override void WriteToBuffer(byte[] buffer, int offset, int length)
        {
            WriteToBufferInvocationCount += 1;

            if (DoesWriteThrow)
            {
                throw new IOException("The test output stream failed.");
            }

            writtenBytes.AddRange(new ArraySegment<byte>(buffer, offset, length));
        }

        internal void QueueReadValues(params int[] values)
        {
            foreach (int value in values)
            {
                readValues.Enqueue(value);
            }
        }
    }
}