using System;
using System.IO;

using NUnit.Framework;

using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    public sealed class MemoryStreamExtensionsTests
    {
        [TestCase(0)]
        [TestCase(4)]
        [TestCase(8)]
        public void GivenAStreamWithData_WhenClearingIt_ThenItsLengthAndPositionAreZero(
            int position)
        {
            using MemoryStream stream = BuildStream(8, position);

            stream.Clear();

            Assert.That(stream.Length, Is.Zero);
            Assert.That(stream.Position, Is.Zero);
        }

        [Test]
        public void GivenAReadOnlyStream_WhenClearingIt_ThenANotSupportedExceptionIsThrown()
        {
            using MemoryStream stream = new(new byte[8], false);

            Assert.That(
                () => stream.Clear(),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GivenADisposedStream_WhenClearingIt_ThenANotSupportedExceptionIsThrown()
        {
            MemoryStream stream = new();
            stream.Dispose();

            Assert.That(
                () => stream.Clear(),
                Throws.TypeOf<NotSupportedException>());
        }

        [Test]
        public void GivenANullStream_WhenClearingIt_ThenANullReferenceExceptionIsThrown()
        {
            MemoryStream stream = null!;

            Assert.That(
                () => stream.Clear(),
                Throws.TypeOf<NullReferenceException>());
        }

        [TestCase(0, 0, 0)]
        [TestCase(8, 0, 8)]
        [TestCase(8, 4, 4)]
        [TestCase(8, 8, 0)]
        [TestCase(8, 16, -8)]
        [TestCase(42, 4, 38)]
        [TestCase(42, 32, 10)]
        public void GivenAStreamPosition_WhenCalculatingTheRemainingLength_ThenTheSignedDifferenceIsReturned(
            int length,
            int position,
            int expectedRemainingLength)
        {
            using MemoryStream stream = BuildStream(length, position);

            int remainingLength = stream.Remaining();

            Assert.That(remainingLength, Is.EqualTo(expectedRemainingLength));
        }

        [Test]
        public void GivenADisposedStream_WhenCalculatingTheRemainingLength_ThenAnObjectDisposedExceptionIsThrown()
        {
            MemoryStream stream = new();
            stream.Dispose();

            Assert.That(
                () => stream.Remaining(),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void GivenANullStream_WhenCalculatingTheRemainingLength_ThenANullReferenceExceptionIsThrown()
        {
            MemoryStream stream = null!;

            Assert.That(
                () => stream.Remaining(),
                Throws.TypeOf<NullReferenceException>());
        }

        private static MemoryStream BuildStream(int length, int position)
        {
            MemoryStream stream = new(new byte[length]);
            stream.Position = position;

            return stream;
        }
    }
}