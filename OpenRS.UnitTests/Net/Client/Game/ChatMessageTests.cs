using System;

using NUnit.Framework;

using OpenRS.Net.Client.Game;

namespace OpenRS.UnitTests.Net.Client.Game
{
    [TestFixture]
    [NonParallelizable]
    public sealed class ChatMessageTests
    {
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

        [Test]
        public void GivenAnInvalidReadRange_WhenDecodingIt_ThenTheFallbackTextIsReturned()
            => Assert.That(
                ChatMessage.BytesToString([42], 8, 16),
                Is.EqualTo("."));
    }
}