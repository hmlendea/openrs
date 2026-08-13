using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    [NonParallelizable]
    public sealed class ChatMessageTests
    {
        [SetUp]
        public void SetUp() => ChatMessage.LastChat = new byte[100];

        [TearDown]
        public void TearDown() => ChatMessage.LastChat = new byte[100];

        [Test]
        public void GivenAChatMessage_WhenEncodingIt_ThenTheCompatibleNibbleBytesAreProduced()
        {
            byte[] expectedBytes = [0x9c, 0x71, 0x8d, 0x23, 0xd6, 0x10];

            int byteCount = ChatMessage.StringToBytes("RuneScape");
            byte[] encodedBytes = ChatMessage.LastChat[..byteCount];

            Assert.That(encodedBytes, Is.EqualTo(expectedBytes));
        }

        [TestCase("hello world", "Hello world")]
        [TestCase("hello. world!", "Hello. World!")]
        [TestCase("dark souls iii", "Dark souls iii ")]
        [TestCase("", "")]
        public void GivenAChatMessage_WhenEncodingAndDecodingIt_ThenTheCompatibleTextIsReturned(
            string message,
            string expectedMessage)
        {
            int byteCount = ChatMessage.StringToBytes(message);
            byte[] encodedBytes = ChatMessage.LastChat[..byteCount];

            string decodedMessage = ChatMessage.BytesToString(encodedBytes, -1, byteCount);

            Assert.That(decodedMessage, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void GivenSignedEncodedBytes_WhenDecodingThem_ThenTheCompatibleTextIsReturned()
        {
            sbyte[] encodedBytes = [unchecked((sbyte)0x9c), 0x71, unchecked((sbyte)0x8d), 0x23, unchecked((sbyte)0xd6), 0x10];

            Assert.That(
                ChatMessage.BytesToString(encodedBytes, -1, encodedBytes.Length),
                Is.EqualTo("Runescape "));
        }

        [Test]
        public void GivenAMessageLongerThanEightyCharacters_WhenEncodingIt_ThenTheMessageIsTruncated()
        {
            string message = new('e', 96);
            string expectedMessage = new string('E', 1) + new string('e', 79);

            int byteCount = ChatMessage.StringToBytes(message);
            byte[] encodedBytes = ChatMessage.LastChat[..byteCount];
            string decodedMessage = ChatMessage.BytesToString(encodedBytes, -1, byteCount);

            Assert.That(decodedMessage, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void GivenAnUnsupportedCharacter_WhenEncodingIt_ThenItIsReplacedWithASpace()
        {
            int byteCount = ChatMessage.StringToBytes("test_42");
            byte[] encodedBytes = ChatMessage.LastChat[..byteCount];

            Assert.That(
                ChatMessage.BytesToString(encodedBytes, -1, byteCount),
                Is.EqualTo("Test 42 "));
        }

        [TestCase("test%value", "Test value")]
        [TestCase("123456@abc", "123456 Abc ")]
        public void GivenAFilteredMarker_WhenDecodingIt_ThenTheMarkerIsReplacedWithASpace(
            string message,
            string expectedMessage)
        {
            int byteCount = ChatMessage.StringToBytes(message);

            string decodedMessage = ChatMessage.BytesToString(ChatMessage.LastChat, -1, byteCount);

            Assert.That(decodedMessage, Is.EqualTo(expectedMessage));
        }

        [Test]
        public void GivenAnAtMarkerWithinTheFirstFiveCharacters_WhenDecodingIt_ThenItIsRetained()
        {
            int byteCount = ChatMessage.StringToBytes("@red@");

            string decodedMessage = ChatMessage.BytesToString(ChatMessage.LastChat, -1, byteCount);

            Assert.That(decodedMessage, Is.EqualTo("@Red@ "));
        }

        [Test]
        public void GivenEncodedBytesAfterAPrefix_WhenDecodingFromThePrefixOffset_ThenOnlyTheMessageIsRead()
        {
            int byteCount = ChatMessage.StringToBytes("RuneScape");
            byte[] encodedBytes = [42, .. ChatMessage.LastChat[..byteCount], 64];

            string decodedMessage = ChatMessage.BytesToString(encodedBytes, 0, byteCount);

            Assert.That(decodedMessage, Is.EqualTo("Runescape "));
        }

        [TestCase(0)]
        [TestCase(-1)]
        [TestCase(int.MinValue)]
        public void GivenANonPositiveByteCount_WhenDecodingNullBytes_ThenAnEmptyMessageIsReturned(
            int byteCount)
            => Assert.That(
                ChatMessage.BytesToString((byte[])null!, 42, byteCount),
                Is.Empty);

        [Test]
        public void GivenNullSignedBytes_WhenDecodingThem_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => ChatMessage.BytesToString((sbyte[])null!, -1, 0),
                Throws.TypeOf<ArgumentNullException>());

        [TestCase(-2, 1)]
        [TestCase(0, 2)]
        [TestCase(int.MaxValue, 1)]
        public void GivenAnInvalidReadRange_WhenDecodingBytes_ThenTheFallbackTextIsReturned(
            int readOffset,
            int byteCount)
            => Assert.That(
                ChatMessage.BytesToString([42], readOffset, byteCount),
                Is.EqualTo("."));

        [Test]
        public void GivenTheMaximumNibblePair_WhenDecodingIt_ThenTheFinalCharacterIsReturned()
            => Assert.That(
                ChatMessage.BytesToString([0xff], -1, 1),
            Is.EqualTo("]"));

        [Test]
        public void GivenNullText_WhenEncodingIt_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => ChatMessage.StringToBytes(null!),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenAnEmptyPublicOutputBuffer_WhenEncodingText_ThenAnIndexExceptionIsThrown()
        {
            ChatMessage.LastChat = [];

            Assert.That(
                () => ChatMessage.StringToBytes("RuneScape"),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]
        public void GivenAnInvalidReadRange_WhenDecodingIt_ThenTheFallbackTextIsReturned()
            => Assert.That(
                ChatMessage.BytesToString([42], 8, 16),
                Is.EqualTo("."));
    }
}