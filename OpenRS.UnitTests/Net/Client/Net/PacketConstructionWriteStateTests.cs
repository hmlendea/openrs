using System;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using OpenRS.Net.Client.Net;

namespace OpenRS.UnitTests.Net.Client.Net
{
    [TestFixture]
    [NonParallelizable]
    public sealed class PacketConstructionWriteStateTests
    {
        private static int CommandIdentifier => 64;

        private PacketConstructionTestDouble packetConstruction = null!;

        [SetUp]
        public void SetUp()
        {
            Array.Clear(
                PacketConstruction.packetCommandCount,
                0,
                PacketConstruction.packetCommandCount.Length);
            Array.Clear(
                PacketConstruction.packetLengthCount,
                0,
                PacketConstruction.packetLengthCount.Length);
            packetConstruction = new PacketConstructionTestDouble();
        }

        [Test]
        public void GivenANewPacketConstruction_WhenReadingItsState_ThenItUsesCompatibleDefaults()
        {
            Assert.That(packetConstruction.length, Is.Zero);
            Assert.That(packetConstruction._read, Is.Zero);
            Assert.That(packetConstruction.maxPacketReadCount, Is.Zero);
            Assert.That(packetConstruction.packetStart, Is.Zero);
            Assert.That(packetConstruction.packetData, Is.Null);
            Assert.That(packetConstruction.maxPacketLength, Is.EqualTo(5000));
            Assert.That(packetConstruction.packetCount, Is.Zero);
            Assert.That(packetConstruction.errorText, Is.Empty);
            Assert.That(packetConstruction.error, Is.False);
            Assert.That(packetConstruction.HasData(), Is.False);
        }

        [TestCase(int.MinValue, 0)]
        [TestCase(-1, 255)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(64, 64)]
        [TestCase(255, 255)]
        [TestCase(256, 0)]
        [TestCase(int.MaxValue, 255)]
        public void GivenACommandIdentifier_WhenCreatingAPacket_ThenItsLowByteIsStored(
            int commandIdentifier,
            int expectedCommandByte)
        {
            packetConstruction.CreatePacket(commandIdentifier);

            Assert.That(packetConstruction.packetData, Has.Length.EqualTo(5000));
            Assert.That(packetConstruction.packetData[2], Is.EqualTo(expectedCommandByte));
        }

        [TestCase(int.MinValue, 0)]
        [TestCase(-1, 255)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(127, 127)]
        [TestCase(255, 255)]
        [TestCase(256, 0)]
        [TestCase(int.MaxValue, 255)]
        public void GivenAByteValue_WhenFormattingAPacket_ThenItsLowByteIsWritten(
            int value,
            int expectedByte)
        {
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddByte(value);

            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(new[] { 2, expectedByte, CommandIdentifier }));
        }

        [TestCase(0, 0, 0)]
        [TestCase(0x1234, 0x12, 0x34)]
        [TestCase(short.MaxValue, 0x7f, 0xff)]
        [TestCase(short.MinValue, 0x80, 0x00)]
        [TestCase(-1, 0xff, 0xff)]
        [TestCase(0x10000, 0x00, 0x00)]
        public void GivenAShortValue_WhenFormattingAPacket_ThenItsLowTwoBytesAreBigEndian(
            int value,
            int expectedHighByte,
            int expectedLowByte)
        {
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddShort(value);

            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(new[] { 3, expectedLowByte, CommandIdentifier, expectedHighByte }));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(0x12345678)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void GivenAnInteger_WhenFormattingAPacket_ThenItsBytesAreBigEndian(int value)
        {
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddInt(value);

            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(BuildCompactPrimitivePacket(GetBigEndianBytes(value))));
        }

        [TestCase(0L)]
        [TestCase(1L)]
        [TestCase(-1L)]
        [TestCase(0x0123456789abcdefL)]
        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        public void GivenALong_WhenFormattingAPacket_ThenItsBytesAreBigEndian(long value)
        {
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddLong(value);

            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(BuildCompactPrimitivePacket(GetBigEndianBytes(value))));
        }

        [TestCase("")]
        [TestCase("RuneScape")]
        [TestCase("Praise the Sun!")]
        [TestCase("România")]
        [TestCase("e=mc²")]
        public void GivenText_WhenFormattingAPacket_ThenItsUtf8BytesAreWritten(string text)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddString(text);

            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(BuildExpectedPacket(textBytes)));
        }

        [Test]
        public void GivenNullText_WhenAddingIt_ThenAnArgumentNullExceptionIsThrown()
        {
            packetConstruction.CreatePacket(CommandIdentifier);

            Assert.That(
                () => packetConstruction.AddString(null!),
                Throws.TypeOf<ArgumentNullException>());
        }

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(1, 3)]
        [TestCase(4, 1)]
        [TestCase(5, 0)]
        public void GivenAByteRange_WhenFormattingAPacket_ThenOnlyThatRangeIsWritten(
            int sourceOffset,
            int byteCount)
        {
            byte[] source = [4, 8, 16, 32, 42];
            byte[] expectedPayload = source.Skip(sourceOffset).Take(byteCount).ToArray();
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddBytes(source, sourceOffset, byteCount);

            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(BuildExpectedPacket(expectedPayload)));
        }

        [Test]
        public void GivenANullByteArrayWithAZeroLength_WhenAddingIt_ThenNoSourceAccessOccurs()
        {
            packetConstruction.CreatePacket(CommandIdentifier);

            packetConstruction.AddBytes(null!, 0, 0);
            packetConstruction.FormatPacket();

            Assert.That(packetConstruction.WrittenBytes, Is.EqualTo(new[] { 1, CommandIdentifier }));
        }

        [TestCase(-1, 1)]
        [TestCase(0, 6)]
        [TestCase(5, 1)]
        public void GivenAnInvalidByteRange_WhenAddingIt_ThenAnIndexExceptionIsThrown(
            int sourceOffset,
            int byteCount)
        {
            packetConstruction.CreatePacket(CommandIdentifier);

            Assert.That(
                () => packetConstruction.AddBytes([4, 8, 16, 32, 42], sourceOffset, byteCount),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [TestCase(0, 2)]
        [TestCase(1, 3)]
        [TestCase(158, 160)]
        [TestCase(159, 162)]
        [TestCase(160, 163)]
        [TestCase(256, 259)]
        public void GivenAPayload_WhenFormattingIt_ThenCommandMetricsRecordTheWireLength(
            int payloadLength,
            int expectedWireLength)
        {
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddBytes(new byte[payloadLength], 0, payloadLength);

            packetConstruction.FormatPacket();

            Assert.That(
                PacketConstruction.packetCommandCount[CommandIdentifier],
                Is.EqualTo(1));
            Assert.That(
                PacketConstruction.packetLengthCount[CommandIdentifier],
                Is.EqualTo(expectedWireLength));
        }

        [Test]
        public void GivenALargePacketBuffer_WhenFormattingIt_ThenCommandMetricsAreNotRecorded()
        {
            packetConstruction.maxPacketLength = 10001;
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddByte(42);

            packetConstruction.FormatPacket();

            Assert.That(PacketConstruction.packetCommandCount[CommandIdentifier], Is.Zero);
            Assert.That(PacketConstruction.packetLengthCount[CommandIdentifier], Is.Zero);
        }

        [Test]
        public void GivenBufferedBytes_WhenWritingBelowTheBatchThreshold_ThenTheyRemainBuffered()
        {
            packetConstruction.packetData = [4, 8, 16, 32];
            packetConstruction.packetStart = 3;

            packetConstruction.WritePacket(2);

            Assert.That(packetConstruction.WrittenBytes, Is.Empty);
            Assert.That(packetConstruction.packetStart, Is.EqualTo(3));
            Assert.That(packetConstruction.packetCount, Is.EqualTo(1));
        }

        [Test]
        public void GivenBufferedBytes_WhenReachingTheBatchThreshold_ThenTheyAreWrittenAndStateIsReset()
        {
            packetConstruction.packetData = [4, 8, 16, 32];
            packetConstruction.packetStart = 3;

            packetConstruction.WritePacket(2);
            packetConstruction.WritePacket(2);

            Assert.That(packetConstruction.WrittenBytes, Is.EqualTo(new[] { 4, 8, 16 }));
            Assert.That(packetConstruction.packetStart, Is.Zero);
            Assert.That(packetConstruction.packetCount, Is.Zero);
        }

        [Test]
        public void GivenRawBufferedBytes_WhenFlushingWithoutFormatting_ThenTheyAreWrittenUnchanged()
        {
            packetConstruction.packetData = [4, 8, 16, 32];
            packetConstruction.packetStart = 3;

            packetConstruction.Flush(false);

            Assert.That(packetConstruction.WrittenBytes, Is.EqualTo(new[] { 4, 8, 16 }));
            Assert.That(packetConstruction.packetStart, Is.Zero);
        }

        [Test]
        public void GivenAPendingPacket_WhenFlushingWithFormatting_ThenItIsFramedAndWritten()
        {
            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddByte(42);

            packetConstruction.Flush();

            Assert.That(packetConstruction.WrittenBytes, Is.EqualTo(new[] { 2, 42, CommandIdentifier }));
        }

        [Test]
        public void GivenAnErrorState_WhenWriting_ThenTheErrorIsClearedAndAnIOExceptionIsThrown()
        {
            packetConstruction.packetData = new byte[8];
            packetConstruction.packetStart = 4;
            packetConstruction.packetCount = 8;
            packetConstruction.error = true;
            packetConstruction.errorText = "The output stream failed.";

            Assert.That(
                () => packetConstruction.WritePacket(0),
                Throws.TypeOf<IOException>()
                    .With.Message.EqualTo("The output stream failed."));
            Assert.That(packetConstruction.packetStart, Is.Zero);
            Assert.That(packetConstruction.packetCount, Is.EqualTo(8));
            Assert.That(packetConstruction.error, Is.False);
        }

        [Test]
        public void GivenANearCapacityBuffer_WhenCreatingAPacket_ThenBufferedBytesAreWrittenFirst()
        {
            packetConstruction.maxPacketLength = 100;
            packetConstruction.packetData = Enumerable.Range(0, 100).Select(value => (byte)value).ToArray();
            packetConstruction.packetStart = 81;

            packetConstruction.CreatePacket(CommandIdentifier);

            Assert.That(packetConstruction.WrittenBytes, Has.Count.EqualTo(81));
            Assert.That(packetConstruction.packetStart, Is.Zero);
            Assert.That(packetConstruction.packetData[2], Is.EqualTo(CommandIdentifier));
        }

        [Test]
        public void GivenAFailingNearCapacityWrite_WhenCreatingAPacket_ThenTheErrorIsDeferred()
        {
            packetConstruction.maxPacketLength = 100;
            packetConstruction.packetData = new byte[100];
            packetConstruction.packetStart = 81;
            packetConstruction.DoesWriteThrow = true;

            packetConstruction.CreatePacket(CommandIdentifier);

            Assert.That(packetConstruction.error);
            Assert.That(packetConstruction.errorText, Does.Contain("test output stream failed"));
            Assert.That(packetConstruction.packetData[83], Is.EqualTo(CommandIdentifier));
        }

        [Test]
        public void GivenAStream_WhenClosingIt_ThenTheVirtualCloseOperationIsInvoked()
        {
            packetConstruction.CloseStream();

            Assert.That(packetConstruction.IsCloseStreamInvoked);
        }

        private static byte[] BuildCompactPrimitivePacket(byte[] payload)
            => BuildExpectedPacket(payload);

        private static byte[] BuildExpectedPacket(byte[] payload)
        {
            int packetLength = payload.Length + 1;

            if (packetLength < 160)
            {
                byte[] expectedBytes = new byte[payload.Length + 2];
                expectedBytes[0] = (byte)packetLength;

                if (payload.Length == 0)
                {
                    expectedBytes[1] = (byte)CommandIdentifier;

                    return expectedBytes;
                }

                expectedBytes[1] = payload[^1];
                expectedBytes[2] = (byte)CommandIdentifier;
                Array.Copy(payload, 0, expectedBytes, 3, payload.Length - 1);

                return expectedBytes;
            }

            byte[] extendedBytes = new byte[payload.Length + 3];
            extendedBytes[0] = (byte)(160 + packetLength / 256);
            extendedBytes[1] = (byte)(packetLength & 0xff);
            extendedBytes[2] = (byte)CommandIdentifier;
            Array.Copy(payload, 0, extendedBytes, 3, payload.Length);

            return extendedBytes;
        }

        private static byte[] GetBigEndianBytes(int value) =>
        [
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value,
        ];

        private static byte[] GetBigEndianBytes(long value) =>
        [
            (byte)(value >> 56),
            (byte)(value >> 48),
            (byte)(value >> 40),
            (byte)(value >> 32),
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value,
        ];
    }
}