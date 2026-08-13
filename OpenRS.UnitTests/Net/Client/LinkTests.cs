using System;
using System.IO;

using NUnit.Framework;

using OpenRS.Net.Client;

namespace OpenRS.UnitTests.Net.Client
{
    [TestFixture]
    [NonParallelizable]
    public sealed class LinkTests
    {
        [Test]
        public void GivenAnEmptyStream_WhenConvertingIt_ThenAnEmptyArrayIsReturned()
        {
            using BinaryReader reader = BuildReader([]);

            Assert.That(Link.StreamToSbyte(reader), Is.Empty);
        }

        [Test]
        public void GivenUnsignedBytes_WhenConvertingThem_ThenTheirSignedBitPatternsArePreserved()
        {
            using BinaryReader reader = BuildReader([0, 1, 127, 128, 255]);

            sbyte[] convertedBytes = Link.StreamToSbyte(reader);

            Assert.That(convertedBytes, Is.EqualTo(new sbyte[] { 0, 1, 127, -128, -1 }));
        }

        [TestCase(0, new sbyte[] { 4, 8, 16, 32, 42 })]
        [TestCase(1, new sbyte[] { 8, 16, 32, 42 })]
        [TestCase(4, new sbyte[] { 42 })]
        [TestCase(5, new sbyte[] { })]
        public void GivenAStreamPosition_WhenConvertingIt_ThenOnlyRemainingBytesAreReturned(
            int position,
            sbyte[] expectedBytes)
        {
            using BinaryReader reader = BuildReader([4, 8, 16, 32, 42]);
            reader.BaseStream.Position = position;

            sbyte[] convertedBytes = Link.StreamToSbyte(reader);

            Assert.That(convertedBytes, Is.EqualTo(expectedBytes));
        }

        [Test]
        public void GivenANullReader_WhenConvertingIt_ThenANullReferenceExceptionIsThrown()
            => Assert.That(
                () => Link.StreamToSbyte(null!),
                Throws.TypeOf<NullReferenceException>());

        [Test]
        public void GivenADisposedReader_WhenConvertingIt_ThenAnObjectDisposedExceptionIsThrown()
        {
            BinaryReader reader = BuildReader([4, 8, 16, 32, 42]);
            reader.Dispose();

            Assert.That(
                () => Link.StreamToSbyte(reader),
                Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void GivenSignedData_WhenRegisteringItInMemory_ThenTheSameArrayIsRetrieved()
        {
            sbyte[] expectedData = [4, 8, 16, 32, 42];

            Link.AddFile("dummy-cert.pem", expectedData);

            Assert.That(Link.GetFile("dummy-cert.pem"), Is.SameAs(expectedData));
        }

        [Test]
        public void GivenAReader_WhenRegisteringItInMemory_ThenItsConvertedDataIsRetrieved()
        {
            using BinaryReader reader = BuildReader([0, 127, 128, 255]);

            Link.AddFile("dummy-cert.pfx", reader);

            Assert.That(
                Link.GetFile("dummy-cert.pfx"),
                Is.EqualTo(new sbyte[] { 0, 127, -128, -1 }));
        }

        private static BinaryReader BuildReader(byte[] data)
            => new(new MemoryStream(data));
    }
}