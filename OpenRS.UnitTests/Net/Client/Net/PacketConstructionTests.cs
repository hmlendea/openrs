using System;
using System.Linq;
using System.Text;

using NUnit.Framework;

namespace OpenRS.UnitTests.Net.Client.Net
{
    [TestFixture]
    public sealed class PacketConstructionTests
    {
        private static int CommandIdentifier => 64;

        private PacketConstructionTestDouble packetConstruction = null!;

        [SetUp]
        public void SetUp()
        {
            packetConstruction = new PacketConstructionTestDouble();
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(158)]
        [TestCase(159)]
        [TestCase(160)]
        [TestCase(161)]
        [TestCase(255)]
        [TestCase(256)]
        [TestCase(4997)]
        public void GivenAPayload_WhenFormattingThePacket_ThenTheWireBytesRemainCompatible(int payloadLength)
        {
            byte[] payload = BuildPayload(payloadLength);
            byte[] expectedBytes = BuildExpectedPacket(payload);

            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddBytes(payload, 0, payload.Length);
            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(expectedBytes));
        }

        [Test]
        public void GivenPrimitiveValues_WhenFormattingThePacket_ThenTheValuesRemainBigEndian()
        {
            byte[] expectedBytes =
            [
                16,
                0xef,
                (byte)CommandIdentifier,
                42,
                0x12,
                0x34,
                0x12,
                0x34,
                0x56,
                0x78,
                0x01,
                0x23,
                0x45,
                0x67,
                0x89,
                0xab,
                0xcd
            ];

            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddByte(42);
            packetConstruction.AddShort(0x1234);
            packetConstruction.AddInt(0x12345678);
            packetConstruction.AddLong(0x0123456789abcdef);
            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(expectedBytes));
        }

        [Test]
        public void GivenMultibyteText_WhenFormattingThePacket_ThenTheTextRemainsUtf8Encoded()
        {
            string text = "RuneScape România";
            byte[] encodedText = Encoding.UTF8.GetBytes(text);
            byte[] expectedBytes = BuildExpectedPacket(encodedText);

            packetConstruction.CreatePacket(CommandIdentifier);
            packetConstruction.AddString(text);
            packetConstruction.FormatPacket();

            Assert.That(
                packetConstruction.WrittenBytes,
                Is.EqualTo(expectedBytes));
        }

        private static byte[] BuildPayload(int payloadLength)
            => Enumerable.Repeat((byte)42, payloadLength).ToArray();

        private static byte[] BuildExpectedPacket(byte[] payload)
        {
            int packetLength = payload.Length + 1;

            if (packetLength < 160)
            {
                return BuildExpectedCompactPacket(payload, packetLength);
            }

            return BuildExpectedExtendedPacket(payload, packetLength);
        }

        private static byte[] BuildExpectedCompactPacket(byte[] payload, int packetLength)
        {
            byte[] expectedBytes = new byte[payload.Length + 2];
            expectedBytes[0] = (byte)packetLength;
            expectedBytes[1] = (byte)CommandIdentifier;

            if (payload.Length == 0)
            {
                return expectedBytes;
            }

            expectedBytes[1] = payload[^1];
            expectedBytes[2] = (byte)CommandIdentifier;
            Array.Copy(payload, 0, expectedBytes, 3, payload.Length - 1);

            return expectedBytes;
        }

        private static byte[] BuildExpectedExtendedPacket(byte[] payload, int packetLength)
        {
            byte[] expectedBytes = new byte[payload.Length + 3];
            expectedBytes[0] = (byte)(160 + packetLength / 256);
            expectedBytes[1] = (byte)(packetLength & 0xff);
            expectedBytes[2] = (byte)CommandIdentifier;
            Array.Copy(payload, 0, expectedBytes, 3, payload.Length);

            return expectedBytes;
        }
    }
}