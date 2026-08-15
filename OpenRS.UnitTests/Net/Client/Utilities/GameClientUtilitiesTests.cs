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
        [TestCase(999, "999")]
        [TestCase(1000, "@cya@1K @whi@(1,000)")]
        [TestCase(1024, "@cya@1K @whi@(1,024)")]
        [TestCase(4096, "@cya@4K @whi@(4,096)")]
        [TestCase(8192, "@cya@8K @whi@(8,192)")]
        [TestCase(999999, "@cya@999K @whi@(999,999)")]
        [TestCase(1000000, "@gre@1 million @whi@(1,000,000)")]
        [TestCase(1048576, "@gre@1 million @whi@(1,048,576)")]
        [TestCase(int.MaxValue, "@gre@2,147 million @whi@(2,147,483,647)")]
        [TestCase(-999, "@cya@-K @whi@(-,999)")]
        [TestCase(-1000, "@cya@-1K @whi@(-1,000)")]
        [TestCase(-1024, "@cya@-1K @whi@(-1,024)")]
        [TestCase(-999999, "@gre@- million @whi@(-,999,999)")]
        [TestCase(-1000000, "@gre@-1 million @whi@(-1,000,000)")]
        [TestCase(int.MinValue, "@gre@-2,147 million @whi@(-2,147,483,648)")]
        public void GivenAnItemCount_WhenFormattingIt_ThenTheCompatibleDisplayTextIsReturned(
            int itemCount,
            string expectedText)
            => Assert.That(
                GameClientUtilities.FormatItemCount(itemCount),
                Is.EqualTo(expectedText));
    }
}