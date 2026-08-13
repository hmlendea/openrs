using System;

using NUnit.Framework;

using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    public sealed class TradeAndDuelStateCompatibilityTests
    {
        [TestCase(TradeAndDuelState.Initial, 0)]
        [TestCase(TradeAndDuelState.Confirm, 1)]
        public void GivenATradeAndDuelState_WhenReadingItsValue_ThenItRemainsCompatible(
            TradeAndDuelState state,
            int expectedValue)
            => Assert.That((int)state, Is.EqualTo(expectedValue));

        [Test]
        public void GivenTheTradeAndDuelStateContract_WhenCountingMembers_ThenItRemainsCompatible()
            => Assert.That(Enum.GetValues<TradeAndDuelState>(), Has.Length.EqualTo(2));
    }
}