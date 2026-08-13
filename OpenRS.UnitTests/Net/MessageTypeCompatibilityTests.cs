using System;

using NUnit.Framework;

using OpenRS.Net;

namespace OpenRS.UnitTests.Net
{
    [TestFixture]
    public sealed class MessageTypeCompatibilityTests
    {
        [TestCase(MessageType.Chat, 2)]
        [TestCase(MessageType.Game, 3)]
        [TestCase(MessageType.GameLocal, 4)]
        [TestCase(MessageType.Quest, 5)]
        [TestCase(MessageType.PrivateMessage, 6)]
        public void GivenAMessageType_WhenReadingItsIdentifier_ThenTheValueRemainsCompatible(
            MessageType messageType,
            int expectedIdentifier)
            => Assert.That(
                (int)messageType,
                Is.EqualTo(expectedIdentifier));

        [Test]
        public void GivenTheMessageTypeContract_WhenCountingItsMembers_ThenNoMemberIsAddedOrRemoved()
            => Assert.That(
                Enum.GetValues<MessageType>(),
                Has.Length.EqualTo(5));
    }
}