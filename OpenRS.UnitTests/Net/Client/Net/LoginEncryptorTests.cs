using System;
using System.Linq;
using System.Numerics;
using System.Text;

using NUnit.Framework;

using OpenRS.Net.Client.Net;

namespace OpenRS.UnitTests.Net.Client.Net
{
    [TestFixture]
    public sealed class LoginEncryptorTests
    {
        private static int BufferSize => 512;

        private LoginEncryptor encryptor = null!;

        [SetUp]
        public void SetUp()
        {
            encryptor = new LoginEncryptor(new byte[BufferSize]);
        }

        [TearDown]
        public void TearDown()
        {
            if (encryptor.Crypto is not null)
            {
                encryptor.Crypto.Dispose();
            }
        }

        [Test]
        public void GivenAKeyBuffer_WhenConstructingAnEncryptor_ThenTheBufferReferenceAndInitialStateArePreserved()
        {
            byte[] keyBuffer = new byte[42];
            LoginEncryptor localEncryptor = new(keyBuffer);

            try
            {
                Assert.That(localEncryptor.packet, Is.SameAs(keyBuffer));
                Assert.That(localEncryptor.offset, Is.Zero);
                Assert.That(localEncryptor.Crypto, Is.Not.Null);
            }
            finally
            {
                localEncryptor.Crypto.Dispose();
            }
        }

        [Test]
        public void GivenANullKeyBuffer_WhenConstructingAnEncryptor_ThenTheNullReferenceIsPreserved()
        {
            LoginEncryptor localEncryptor = new(null!);

            try
            {
                Assert.That(localEncryptor.packet, Is.Null);
                Assert.That(localEncryptor.offset, Is.Zero);
            }
            finally
            {
                localEncryptor.Crypto.Dispose();
            }
        }

        [TestCase(int.MinValue, 0)]
        [TestCase(-1, 255)]
        [TestCase(0, 0)]
        [TestCase(1, 1)]
        [TestCase(127, 127)]
        [TestCase(255, 255)]
        [TestCase(256, 0)]
        [TestCase(int.MaxValue, 255)]
        public void GivenAByteValue_WhenAddingIt_ThenItsLowByteIsStoredAndTheCursorAdvances(
            int value,
            int expectedByte)
        {
            encryptor.offset = 4;

            encryptor.AddByte(value);

            Assert.That(encryptor.packet[4], Is.EqualTo(expectedByte));
            Assert.That(encryptor.offset, Is.EqualTo(5));
        }

        [Test]
        public void GivenAFullBuffer_WhenAddingAByte_ThenAnIndexExceptionIsThrown()
        {
            encryptor.offset = encryptor.packet.Length;

            Assert.That(
                () => encryptor.AddByte(42),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(0x12345678)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void GivenAnInteger_WhenAddingIt_ThenFourBigEndianBytesAreStored(int value)
        {
            encryptor.offset = 4;

            encryptor.AddInt(value);

            Assert.That(
                encryptor.packet[4..8],
                Is.EqualTo(new[]
                {
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)value,
                }));
            Assert.That(encryptor.offset, Is.EqualTo(8));
        }

        [TestCase("")]
        [TestCase("Angetenar")]
        [TestCase("NucileRullz!")]
        [TestCase("România")]
        [TestCase("e=mc²")]
        public void GivenText_WhenAddingIt_ThenUtf8BytesAndALineFeedAreStored(string text)
        {
            byte[] expectedTextBytes = Encoding.UTF8.GetBytes(text);
            encryptor.offset = 4;

            encryptor.AddString(text);

            Assert.That(
                encryptor.packet[4..(4 + expectedTextBytes.Length)],
                Is.EqualTo(expectedTextBytes));
            Assert.That(encryptor.packet[4 + expectedTextBytes.Length], Is.EqualTo(10));
            Assert.That(encryptor.offset, Is.EqualTo(5 + expectedTextBytes.Length));
        }

        [Test]
        public void GivenNullText_WhenAddingIt_ThenAnArgumentNullExceptionIsThrown()
            => Assert.That(
                () => encryptor.AddString(null!),
                Throws.TypeOf<ArgumentNullException>());

        [TestCase(0, 0)]
        [TestCase(0, 5)]
        [TestCase(1, 3)]
        [TestCase(4, 1)]
        [TestCase(5, 0)]
        public void GivenAByteRange_WhenAddingIt_ThenOnlyThatRangeIsStored(
            int sourceOffset,
            int byteCount)
        {
            byte[] source = [4, 8, 16, 32, 42];
            byte[] expectedBytes = source.Skip(sourceOffset).Take(byteCount).ToArray();
            encryptor.offset = 8;

            encryptor.AddBytes(source, sourceOffset, byteCount);

            Assert.That(encryptor.packet[8..(8 + byteCount)], Is.EqualTo(expectedBytes));
            Assert.That(encryptor.offset, Is.EqualTo(8 + byteCount));
        }

        [Test]
        public void GivenANullByteArrayWithAZeroLength_WhenAddingIt_ThenNoSourceAccessOccurs()
        {
            encryptor.offset = 8;

            encryptor.AddBytes(null!, 0, 0);

            Assert.That(encryptor.offset, Is.EqualTo(8));
        }

        [TestCase(-1, 1)]
        [TestCase(0, 6)]
        [TestCase(5, 1)]
        public void GivenAnInvalidByteRange_WhenAddingIt_ThenAnIndexExceptionIsThrown(
            int sourceOffset,
            int byteCount)
        {
            Assert.That(
                () => encryptor.AddBytes([4, 8, 16, 32, 42], sourceOffset, byteCount),
                Throws.TypeOf<IndexOutOfRangeException>());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(127)]
        [TestCase(128)]
        [TestCase(255)]
        public void GivenAStoredByte_WhenReadingIt_ThenItsUnsignedValueIsReturned(int value)
        {
            encryptor.packet[4] = (byte)value;
            encryptor.offset = 4;

            int actualValue = encryptor.GetByte();

            Assert.That(actualValue, Is.EqualTo(value));
            Assert.That(encryptor.offset, Is.EqualTo(5));
        }

        [TestCase(0x00, 0x00, 0x0000)]
        [TestCase(0x00, 0xff, 0x00ff)]
        [TestCase(0x12, 0x34, 0x1234)]
        [TestCase(0x7f, 0xff, 0x7fff)]
        [TestCase(0xff, 0xff, 0xffff)]
        public void GivenTwoStoredBytes_WhenReadingAShort_ThenTheyAreCombinedBigEndian(
            int highByte,
            int lowByte,
            int expectedValue)
        {
            encryptor.packet[4] = (byte)highByte;
            encryptor.packet[5] = (byte)lowByte;
            encryptor.offset = 4;

            int actualValue = encryptor.GetShort();

            Assert.That(actualValue, Is.EqualTo(expectedValue));
            Assert.That(encryptor.offset, Is.EqualTo(6));
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(0x12345678)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void GivenFourStoredBytes_WhenReadingAnInteger_ThenTheyAreCombinedBigEndian(int value)
        {
            encryptor.packet[4] = (byte)(value >> 24);
            encryptor.packet[5] = (byte)(value >> 16);
            encryptor.packet[6] = (byte)(value >> 8);
            encryptor.packet[7] = (byte)value;
            encryptor.offset = 4;

            int actualValue = encryptor.GetInt();

            Assert.That(actualValue, Is.EqualTo(value));
            Assert.That(encryptor.offset, Is.EqualTo(8));
        }

        [Test]
        public void GivenStoredBytesAndAnOutputOffset_WhenReadingThem_ThenCurrentBytesAreCopiedToThatRange()
        {
            encryptor.packet = [4, 8, 16, 32, 42];
            encryptor.offset = 1;
            byte[] outputBuffer = [96, 96, 96, 96, 96];

            encryptor.GetBytes(outputBuffer, 1, 3);

            Assert.That(outputBuffer, Is.EqualTo(new byte[] { 96, 8, 16, 32, 96 }));
            Assert.That(encryptor.offset, Is.EqualTo(4));
        }

        [Test]
        public void GivenANullOutputBufferWithAZeroLength_WhenReadingBytes_ThenNoOutputAccessOccurs()
        {
            encryptor.offset = 8;

            encryptor.GetBytes(null!, 0, 0);

            Assert.That(encryptor.offset, Is.EqualTo(8));
        }

        [Test]
        public void GivenText_WhenInvokingTheLegacyEncryptMethod_ThenTheSameReferenceIsReturned()
        {
            byte[] text = [4, 8, 16, 32, 42];

            byte[] encryptedText = encryptor.Encrypt(text);

            Assert.That(encryptedText, Is.SameAs(text));
        }

        [Test]
        public void GivenNullText_WhenInvokingTheLegacyEncryptMethod_ThenNullIsReturned()
            => Assert.That(encryptor.Encrypt(null!), Is.Null);

        [Test]
        public void GivenAKnownRsaVector_WhenEncryptingThePacket_ThenTheCiphertextIsBigEndianAndLengthPrefixed()
        {
            encryptor.AddByte(65);

            encryptor.EncryptPacket(new BigInteger(17), new BigInteger(3233));

            Assert.That(encryptor.packet[..3], Is.EqualTo(new byte[] { 2, 0x0a, 0xe6 }));
            Assert.That(encryptor.offset, Is.EqualTo(3));
        }

        [TestCase(new byte[] { 0 }, 1, 3233)]
        [TestCase(new byte[] { 1 }, 17, 3233)]
        [TestCase(new byte[] { 65 }, 17, 3233)]
        [TestCase(new byte[] { 1, 2 }, 7, 65537)]
        [TestCase(new byte[] { 4, 8, 16, 32, 42 }, 3, 2147483647)]
        public void GivenPlaintextAndModularParameters_WhenEncryptingThePacket_ThenTheExpectedValueIsEncoded(
            byte[] plaintext,
            int exponent,
            int modulus)
        {
            encryptor.AddBytes(plaintext, 0, plaintext.Length);
            byte[] expectedCiphertext = CalculateCiphertext(
                plaintext,
                new BigInteger(exponent),
                new BigInteger(modulus));

            encryptor.EncryptPacket(new BigInteger(exponent), new BigInteger(modulus));

            Assert.That(encryptor.packet[0], Is.EqualTo(expectedCiphertext.Length));
            Assert.That(encryptor.packet[1..(expectedCiphertext.Length + 1)], Is.EqualTo(expectedCiphertext));
            Assert.That(encryptor.offset, Is.EqualTo(expectedCiphertext.Length + 1));
        }

        [Test]
        public void GivenAHighBitCiphertext_WhenEncryptingThePacket_ThenNoSignPaddingByteIsEmitted()
        {
            encryptor.AddByte(0x80);

            encryptor.EncryptPacket(BigInteger.One, new BigInteger(257));

            Assert.That(encryptor.packet[..2], Is.EqualTo(new byte[] { 1, 0x80 }));
            Assert.That(encryptor.offset, Is.EqualTo(2));
        }

        [Test]
        public void GivenAnEmptyPlaintext_WhenEncryptingThePacket_ThenAZeroCiphertextIsEmitted()
        {
            encryptor.EncryptPacket(new BigInteger(17), new BigInteger(3233));

            Assert.That(encryptor.packet[..2], Is.EqualTo(new byte[] { 1, 0 }));
            Assert.That(encryptor.offset, Is.EqualTo(2));
        }

        [Test]
        public void GivenANegativeExponent_WhenEncryptingThePacket_ThenAnArgumentExceptionIsThrown()
        {
            encryptor.AddByte(42);

            Assert.That(
                () => encryptor.EncryptPacket(new BigInteger(-1), new BigInteger(3233)),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void GivenAZeroModulus_WhenEncryptingThePacket_ThenADivideByZeroExceptionIsThrown()
        {
            encryptor.AddByte(42);

            Assert.That(
                () => encryptor.EncryptPacket(new BigInteger(17), BigInteger.Zero),
                Throws.TypeOf<DivideByZeroException>());
        }

        private static byte[] CalculateCiphertext(
            byte[] plaintext,
            BigInteger exponent,
            BigInteger modulus)
        {
            BigInteger plaintextValue = new(plaintext, true, true);
            BigInteger ciphertextValue = BigInteger.ModPow(plaintextValue, exponent, modulus);
            byte[] ciphertext = ciphertextValue.ToByteArray(true, true);

            if (ciphertext.Length == 0)
            {
                return [0];
            }

            return ciphertext;
        }
    }
}