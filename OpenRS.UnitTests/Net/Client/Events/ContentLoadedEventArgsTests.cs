using NUnit.Framework;

using OpenRS.Net.Client.Events;

namespace OpenRS.UnitTests.Net.Client.Events
{
    [TestFixture]
    public sealed class ContentLoadedEventArgsTests
    {
        [TestCase("", "", 0.0, 0.0)]
        [TestCase("Loading RuneScape", "RuneScape loaded", 3.14, 100.0)]
        [TestCase("Praise the Sun!", "Jolly cooperation!", -61.3, 87.3)]
        public void GivenContentProgress_WhenConstructingAndUpdatingIt_ThenValuesAreRetained(
            string initialStatus,
            string updatedStatus,
            decimal initialProgress,
            decimal updatedProgress)
        {
            ContentLoadedEventArgs eventArgs = new(initialStatus, initialProgress);

            Assert.That(eventArgs.StatusText, Is.EqualTo(initialStatus));
            Assert.That(eventArgs.Progress, Is.EqualTo(initialProgress));

            eventArgs.StatusText = updatedStatus;
            eventArgs.Progress = updatedProgress;

            Assert.That(eventArgs.StatusText, Is.EqualTo(updatedStatus));
            Assert.That(eventArgs.Progress, Is.EqualTo(updatedProgress));
        }

        [Test]
        public void GivenNullStatusText_WhenConstructingAndUpdatingIt_ThenNullIsRetained()
        {
            ContentLoadedEventArgs eventArgs = new(null!, decimal.MinValue)
            {
                StatusText = null!,
                Progress = decimal.MaxValue,
            };

            Assert.That(eventArgs.StatusText, Is.Null);
            Assert.That(eventArgs.Progress, Is.EqualTo(decimal.MaxValue));
        }
    }
}