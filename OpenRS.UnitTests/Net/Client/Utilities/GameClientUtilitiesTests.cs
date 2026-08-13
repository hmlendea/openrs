using NUnit.Framework;

using OpenRS.Net.Client.Utilities;

namespace OpenRS.UnitTests.Net.Client.Utilities
{
    [TestFixture]
    public sealed class GameClientUtilitiesTests
    {
        [TestCase(0, "0")]
        [TestCase(42, "42")]
        [TestCase(512, "512")]
        [TestCase(1024, "@cya@1K @whi@(1,024)")]
        [TestCase(4096, "@cya@4K @whi@(4,096)")]
        [TestCase(8192, "@cya@8K @whi@(8,192)")]
        [TestCase(1048576, "@gre@1 million @whi@(1,048,576)")]
        [TestCase(-1024, "@cya@-1K @whi@(-1,024)")]
        public void GivenAnItemCount_WhenFormattingIt_ThenTheCompatibleDisplayTextIsReturned(
            int itemCount,
            string expectedText)
            => Assert.That(
                GameClientUtilities.FormatItemCount(itemCount),
                Is.EqualTo(expectedText));
    }
}