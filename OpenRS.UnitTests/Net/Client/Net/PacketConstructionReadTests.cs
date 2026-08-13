using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace OpenRS.UnitTests.Net.Client.Net
{
    [TestFixture]
    public sealed class PacketConstructionReadTests
    {
        private PacketConstructionTestDouble packetConstruction = null!;

        [SetUp]
        public void SetUp()
        {
            packetConstruction = new PacketConstructionTestDouble();
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(255)]
        [TestCase(256)]
        [TestCase(int.MaxValue)]
        public void GivenAReadValue_WhenReadingAByte_ThenTheVirtualValueIsReturnedUnchanged(int value)
        {
            packetConstruction.QueueReadValues(value);

            Assert.That(packetConstruction.ReadByte(), Is.EqualTo(value));
        }

        [TestCase(0x00, 0x00, 0x0000)]
        [TestCase(0x00, 0xff, 0x00ff)]
        [TestCase(0x12, 0x34, 0x1234)]
        [TestCase(0x7f, 0xff, 0x7fff)]
        [TestCase(0xff, 0xff, 0xffff)]
        public void GivenTwoReadBytes_WhenReadingAShort_ThenTheyAreCombinedBigEndian(
            int highByte,
            int lowByte,
            int expectedValue)
        {
            packetConstruction.QueueReadValues(highByte, lowByte);

            Assert.That(packetConstruction.ReadShort(), Is.EqualTo(expectedValue));
        }

        [Test]
        public void GivenEightReadBytes_WhenReadingALong_ThenTheyAreCombinedBigEndian()
        {
            packetConstruction.QueueReadValues(0x01, 0x23, 0x45, 0x67, 0x89, 0xab, 0xcd, 0xef);

            Assert.That(packetConstruction.ReadLong(), Is.EqualTo(0x0123456789abcdefL));
        }

        [Test]
        public void GivenAReadRequest_WhenReadingIntoABuffer_ThenTypeZeroIsForwarded()
        {
            sbyte[] buffer = new sbyte[4];
            packetConstruction.QueueReadValues(4, 8, 16, 32);

            packetConstruction.Read(buffer.Length, buffer);

            Assert.That(buffer, Is.EqualTo(new sbyte[] { 4, 8, 16, 32 }));
            Assert.That(packetConstruction.LastReadSize, Is.EqualTo(4));
            Assert.That(packetConstruction.LastReadType, Is.Zero);
            Assert.That(packetConstruction.ReadInputStreamInvocationCount, Is.EqualTo(1));
        }

        [TestCase(new[] { 1, 64 }, new[] { 64 })]
        [TestCase(new[] { 2, 42, 64 }, new[] { 64, 42 })]
        [TestCase(new[] { 4, 32, 64, 4, 8 }, new[] { 64, 4, 8, 32 })]
        public void GivenACompactWirePacket_WhenReadingIt_ThenTheSwappedByteIsRestored(
            int[] wireBytes,
            int[] expectedPacketBytes)
        {
            sbyte[] packetBuffer = new sbyte[expectedPacketBytes.Length];
            packetConstruction.QueueReadValues(wireBytes);

            int packetLength = packetConstruction.ReadPacket(packetBuffer);

            Assert.That(packetLength, Is.EqualTo(expectedPacketBytes.Length));
            Assert.That(
                packetBuffer,
                Is.EqualTo(expectedPacketBytes.Select(value => (sbyte)value).ToArray()));
            Assert.That(packetConstruction.length, Is.Zero);
            Assert.That(packetConstruction._read, Is.Zero);
        }

        [TestCase(160)]
        [TestCase(161)]
        [TestCase(255)]
        [TestCase(256)]
        [TestCase(512)]
        public void GivenAnExtendedWirePacket_WhenReadingIt_ThenEveryPacketByteIsPreserved(int packetLength)
        {
            int[] packetBytes = Enumerable.Range(0, packetLength)
                .Select(value => value & 0xff)
                .ToArray();
            int[] wireBytes =
            [
                160 + packetLength / 256,
                packetLength & 0xff,
                .. packetBytes,
            ];
            sbyte[] packetBuffer = new sbyte[packetLength];
            packetConstruction.QueueReadValues(wireBytes);

            int actualPacketLength = packetConstruction.ReadPacket(packetBuffer);

            Assert.That(actualPacketLength, Is.EqualTo(packetLength));
            Assert.That(
                packetBuffer,
                Is.EqualTo(packetBytes.Select(value => unchecked((sbyte)value)).ToArray()));
            Assert.That(packetConstruction.LastReadSize, Is.EqualTo(packetLength));
            Assert.That(packetConstruction.length, Is.Zero);
            Assert.That(packetConstruction._read, Is.Zero);
        }

        [Test]
        public void GivenConsecutiveCompactAndExtendedPackets_WhenReadingThem_ThenStateDoesNotLeak()
        {
            int[] extendedPacketBytes = Enumerable.Range(0, 160).Select(value => value & 0xff).ToArray();
            int[] wireBytes =
            [
                2,
                42,
                64,
                160,
                160,
                .. extendedPacketBytes,
            ];
            packetConstruction.QueueReadValues(wireBytes);
            sbyte[] compactBuffer = new sbyte[2];
            sbyte[] extendedBuffer = new sbyte[160];

            int compactLength = packetConstruction.ReadPacket(compactBuffer);
            int extendedLength = packetConstruction.ReadPacket(extendedBuffer);

            Assert.That(compactLength, Is.EqualTo(2));
            Assert.That(compactBuffer, Is.EqualTo(new sbyte[] { 64, 42 }));
            Assert.That(extendedLength, Is.EqualTo(160));
            Assert.That(
                extendedBuffer,
                Is.EqualTo(extendedPacketBytes.Select(value => unchecked((sbyte)value)).ToArray()));
        }

        [Test]
        public void GivenRepeatedIncompleteReads_WhenExceedingTheReadLimit_ThenATimeoutIsDeferred()
        {
            packetConstruction.maxPacketReadCount = 2;
            packetConstruction.QueueReadValues(0, 0, 0, 0);

            Assert.That(packetConstruction.ReadPacket(new sbyte[1]), Is.Zero);
            Assert.That(packetConstruction.ReadPacket(new sbyte[1]), Is.Zero);
            Assert.That(packetConstruction.ReadPacket(new sbyte[1]), Is.Zero);

            Assert.That(packetConstruction.error);
            Assert.That(packetConstruction.errorText, Is.EqualTo("time-out"));
            Assert.That(packetConstruction.maxPacketReadCount, Is.EqualTo(4));
            Assert.That(packetConstruction._read, Is.EqualTo(3));
        }

        [Test]
        public void GivenATimedOutRead_WhenWritingNext_ThenAnIOExceptionIsThrownAndTheErrorClears()
        {
            packetConstruction.maxPacketReadCount = 1;
            packetConstruction.QueueReadValues(0, 0);
            packetConstruction.ReadPacket(new sbyte[1]);
            packetConstruction.ReadPacket(new sbyte[1]);

            Assert.That(
                () => packetConstruction.WritePacket(0),
                Throws.TypeOf<IOException>()
                    .With.Message.EqualTo("time-out"));
            Assert.That(packetConstruction.error, Is.False);
            Assert.That(packetConstruction.packetStart, Is.Zero);
        }

        [Test]
        public void GivenAnInputIOException_WhenReadingAPacket_ThenTheErrorIsDeferred()
        {
            packetConstruction.DoesReadThrow = true;

            int packetLength = packetConstruction.ReadPacket(new sbyte[8]);

            Assert.That(packetLength, Is.Zero);
            Assert.That(packetConstruction.error);
            Assert.That(packetConstruction.errorText, Does.Contain("test input stream failed"));
        }

        [Test]
        public void GivenACompactPacketAndAnUndersizedBuffer_WhenReadingIt_ThenAnIndexExceptionIsThrown()
        {
            packetConstruction.QueueReadValues(2, 42, 64);

            Assert.That(
                () => packetConstruction.ReadPacket(new sbyte[1]),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenAnExtendedPacketAndAnUndersizedBuffer_WhenReadingIt_ThenAnIndexExceptionIsThrown()
        {
            packetConstruction.QueueReadValues(160, 2, 4, 8);

            Assert.That(
                () => packetConstruction.ReadPacket(new sbyte[1]),
                Throws.TypeOf<IndexOutOfRangeException>());
        }
    }
}