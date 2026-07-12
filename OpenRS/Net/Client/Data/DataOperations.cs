using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using OpenRS.Settings;

namespace OpenRS.Net.Client.Data
{
    public sealed class DataOperations
    {
        public static Uri CodeBase = null;

        private static readonly int[] bitMask = [0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 65535, 0x1ffff, 0x3ffff, 0x7ffff, 0xfffff, 0x1fffff, 0x3fffff, 0x7fffff, 0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff, 0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff, -1];

        public static MemoryStream OpenInputStream(string fileName)
        {
            Stream inputStream;

            if (CodeBase is null)
            {
                inputStream = File.OpenRead(Path.Combine(Config.ConfigurationDirectory, fileName));
            }
            else
            {
                Uri url = new(CodeBase, fileName);
                WebRequest webRequest = WebRequest.Create(url.ToString());
                inputStream = webRequest.GetResponse().GetResponseStream();
            }

            MemoryStream memory = new();
            int bytesRead = 0;
            byte[] buffer = new byte[2048];

            while ((bytesRead = inputStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                memory.Write((byte[])(Array)buffer, 0, bytesRead);
            }

            return memory;
        }

        public static void ReadFully(string path, sbyte[] data, int length)
        {
            using MemoryStream stream = OpenInputStream(path);
            sbyte[] readData = StreamToSbyte(new BinaryReader(stream), length);
            Array.Copy(readData, data, readData.Length);
        }

        public static void ReadFully(string path, byte[] data, int length)
        {
            using MemoryStream stream = OpenInputStream(path);
            stream.Read(data, 0, length);
        }

        public static int GetByte(sbyte value) => value & 0xff;

        public static int GetShort(sbyte[] data, int offset) =>
            ((data[offset] & 0xff) << 8) + (data[offset + 1] & 0xff);

        public static int GetInt16(sbyte[] data, int offset) => GetShort(data, offset);

        public static long GetLong(sbyte[] data, int offset) =>
            (((long)GetInt(data, offset) & 0xffffffffL) << 32) + ((long)GetInt(data, offset + 4) & 0xffffffffL);

        public static long GetLong(byte[] data, int offset) =>
            (((long)GetInt(data, offset) & 0xffffffffL) << 32) + ((long)GetInt(data, offset + 4) & 0xffffffffL);

        public static int GetInt(sbyte[] data, int offset) =>
            ((data[offset] & 0xff) << 24) + ((data[offset + 1] & 0xff) << 16) + ((data[offset + 2] & 0xff) << 8) + (data[offset + 3] & 0xff);

        public static int GetInt(byte[] data, int offset) =>
            ((data[offset] & 0xff) << 24) + ((data[offset + 1] & 0xff) << 16) + ((data[offset + 2] & 0xff) << 8) + (data[offset + 3] & 0xff);

        public static int GetSignedShort(sbyte[] data, int offset)
        {
            int value = GetByte(data[offset]) * 256 + GetByte(data[offset + 1]);

            if (value > 32767)
            {
                value -= 0x10000;
            }

            return value;
        }

        public static int GetBits(sbyte[] data, int offset, int bitCount)
        {
            int byteOffset = offset >> 3;
            int bitModifier = 8 - (offset & 7);
            int result = 0;

            for (; bitCount > bitModifier; bitModifier = 8)
            {
                result += (data[byteOffset] & bitMask[bitModifier]) << bitCount - bitModifier;
                byteOffset += 1;
                bitCount -= bitModifier;
            }

            if (bitCount == bitModifier)
            {
                result += data[byteOffset] & bitMask[bitModifier];
            }
            else
            {
                result += data[byteOffset] >> bitModifier - bitCount & bitMask[bitCount];
            }

            return result;
        }

        public static string FormatString(string text, int maxLength)
        {
            string result = "";

            for (int charIndex = 0; charIndex < maxLength; charIndex += 1)
            {
                if (charIndex >= text.Length)
                {
                    result += " ";
                }
                else
                {
                    char character = text[charIndex];

                    if (character >= 'a' && character <= 'z')
                    {
                        result += character;
                    }
                    else if (character >= 'A' && character <= 'Z')
                    {
                        result += character;
                    }
                    else if (character >= '0' && character <= '9')
                    {
                        result += character;
                    }
                    else
                    {
                        result += '_';
                    }
                }
            }

            return result;
        }

        public static string IpToString(int ipAddress) =>
            (ipAddress >> 24 & 0xff) + "." + (ipAddress >> 16 & 0xff) + "." + (ipAddress >> 8 & 0xff) + "." + (ipAddress & 0xff);

        public static long NameToHash(string name)
        {
            string normalizedName = "";

            for (int charIndex = 0; charIndex < name.Length; charIndex += 1)
            {
                char character = name[charIndex];

                if (character >= 'a' && character <= 'z')
                {
                    normalizedName += character;
                }
                else if (character >= 'A' && character <= 'Z')
                {
                    normalizedName += (char)((character + 97) - 65);
                }
                else if (character >= '0' && character <= '9')
                {
                    normalizedName += character;
                }
                else
                {
                    normalizedName += ' ';
                }
            }

            normalizedName = normalizedName.Trim();

            if (normalizedName.Length > 12)
            {
                normalizedName = normalizedName.Substring(0, 12);
            }

            long hashValue = 0L;

            for (int charIndex = 0; charIndex < normalizedName.Length; charIndex += 1)
            {
                char character = normalizedName[charIndex];
                hashValue *= 37L;

                if (character >= 'a' && character <= 'z')
                {
                    hashValue += (1 + character) - 97;
                }
                else if (character >= '0' && character <= '9')
                {
                    hashValue += (27 + character) - 48;
                }
            }

            return hashValue;
        }

        public static string HashToName(long hash)
        {
            if (hash < 0L)
            {
                return "invalid_name";
            }

            string name = "";

            while (hash != 0L)
            {
                int remainder = (int)(hash % 37L);
                hash /= 37L;

                if (remainder == 0)
                {
                    name = " " + name;
                }
                else if (remainder < 27)
                {
                    if (hash % 37L == 0L)
                    {
                        name = (char)((remainder + 65) - 1) + name;
                    }
                    else
                    {
                        name = (char)((remainder + 97) - 1) + name;
                    }
                }
                else
                {
                    name = (char)((remainder + 48) - 27) + name;
                }
            }

            return name;
        }

        public static long GetObjectOffset(string objectName, sbyte[] objectData)
        {
            int entryCount = GetShort(objectData, 0);
            int nameHash = 0;
            objectName = objectName.ToUpper();

            for (int charIndex = 0; charIndex < objectName.Length; charIndex += 1)
            {
                nameHash = (nameHash * 61 + objectName[charIndex]) - 32;
            }

            long dataOffset = 2 + entryCount * 10;

            for (int entryIndex = 0; entryIndex < entryCount; entryIndex += 1)
            {
                long entryNameHash = (objectData[entryIndex * 10 + 2] & 0xff) * 0x1000000 + (objectData[entryIndex * 10 + 3] & 0xff) * 0x10000 + (objectData[entryIndex * 10 + 4] & 0xff) * 256 + (objectData[entryIndex * 10 + 5] & 0xff);
                long entrySize = (objectData[entryIndex * 10 + 9] & 0xff) * 0x10000 + (objectData[entryIndex * 10 + 10] & 0xff) * 256 + (objectData[entryIndex * 10 + 11] & 0xff);

                if (entryNameHash == nameHash)
                {
                    return dataOffset;
                }

                dataOffset += entrySize;
            }

            return 0;
        }

        public static int GetSoundLength(string soundName, sbyte[] soundIndex)
        {
            int entryCount = GetShort(soundIndex, 0);
            int nameHash = 0;
            soundName = soundName.ToUpper();

            for (int charIndex = 0; charIndex < soundName.Length; charIndex += 1)
            {
                nameHash = (nameHash * 61 + soundName[charIndex]) - 32;
            }

            for (int entryIndex = 0; entryIndex < entryCount; entryIndex += 1)
            {
                int entryNameHash = (soundIndex[entryIndex * 10 + 2] & 0xff) * 0x1000000 + (soundIndex[entryIndex * 10 + 3] & 0xff) * 0x10000 + (soundIndex[entryIndex * 10 + 4] & 0xff) * 256 + (soundIndex[entryIndex * 10 + 5] & 0xff);
                int entryLength = (soundIndex[entryIndex * 10 + 6] & 0xff) * 0x10000 + (soundIndex[entryIndex * 10 + 7] & 0xff) * 256 + (soundIndex[entryIndex * 10 + 8] & 0xff);

                if (entryNameHash == nameHash)
                {
                    return entryLength;
                }
            }

            return 0;
        }

        public static byte[] LoadData(string entryName, int outputOffset, byte[] indexData) =>
            LoadData(entryName, outputOffset, indexData, null);

        public static byte[] LoadData(string entryName, int outputOffset, byte[] indexData, byte[] outputBuffer)
        {
            int entryCount = (indexData[0] & 0xff) * 256 + (indexData[1] & 0xff);
            int nameHash = 0;
            entryName = entryName.ToUpper();

            for (int charIndex = 0; charIndex < entryName.Length; charIndex += 1)
            {
                nameHash = (nameHash * 61 + entryName[charIndex]) - 32;
            }

            int dataOffset = 2 + entryCount * 10;

            for (int entryIndex = 0; entryIndex < entryCount; entryIndex += 1)
            {
                int entryNameHash = (indexData[entryIndex * 10 + 2] & 0xff) * 0x1000000 + (indexData[entryIndex * 10 + 3] & 0xff) * 0x10000 + (indexData[entryIndex * 10 + 4] & 0xff) * 256 + (indexData[entryIndex * 10 + 5] & 0xff);
                int decompressedSize = (indexData[entryIndex * 10 + 6] & 0xff) * 0x10000 + (indexData[entryIndex * 10 + 7] & 0xff) * 256 + (indexData[entryIndex * 10 + 8] & 0xff);
                int compressedSize = (indexData[entryIndex * 10 + 9] & 0xff) * 0x10000 + (indexData[entryIndex * 10 + 10] & 0xff) * 256 + (indexData[entryIndex * 10 + 11] & 0xff);

                if (entryNameHash == nameHash)
                {
                    if (outputBuffer is null)
                    {
                        outputBuffer = new byte[decompressedSize + outputOffset];
                    }

                    if (decompressedSize != compressedSize)
                    {
                        DataFileDecrypter.UnpackData(outputBuffer, decompressedSize, indexData, compressedSize, dataOffset);
                    }
                    else
                    {
                        for (long byteIndex = 0; byteIndex < decompressedSize; byteIndex += 1)
                        {
                            outputBuffer[byteIndex] = indexData[dataOffset + byteIndex];
                        }
                    }

                    return outputBuffer;
                }

                dataOffset += compressedSize;
            }

            return null;
        }

        public static sbyte[] LoadData(string entryName, int outputOffset, sbyte[] indexData) =>
            LoadData(entryName, outputOffset, indexData, null);

        public static sbyte[] LoadData(string entryName, int outputOffset, sbyte[] indexData, sbyte[] outputBuffer)
        {
            int entryCount = (indexData[0] & 0xff) * 256 + (indexData[1] & 0xff);
            int nameHash = 0;
            entryName = entryName.ToUpper();

            for (int charIndex = 0; charIndex < entryName.Length; charIndex += 1)
            {
                nameHash = (nameHash * 61 + entryName[charIndex]) - 32;
            }

            int dataOffset = 2 + entryCount * 10;

            for (int entryIndex = 0; entryIndex < entryCount; entryIndex += 1)
            {
                int entryNameHash = (indexData[entryIndex * 10 + 2] & 0xff) * 0x1000000 + (indexData[entryIndex * 10 + 3] & 0xff) * 0x10000 + (indexData[entryIndex * 10 + 4] & 0xff) * 256 + (indexData[entryIndex * 10 + 5] & 0xff);
                int decompressedSize = (indexData[entryIndex * 10 + 6] & 0xff) * 0x10000 + (indexData[entryIndex * 10 + 7] & 0xff) * 256 + (indexData[entryIndex * 10 + 8] & 0xff);
                int compressedSize = (indexData[entryIndex * 10 + 9] & 0xff) * 0x10000 + (indexData[entryIndex * 10 + 10] & 0xff) * 256 + (indexData[entryIndex * 10 + 11] & 0xff);

                if (entryNameHash == nameHash)
                {
                    if (outputBuffer is null)
                    {
                        outputBuffer = new sbyte[decompressedSize + outputOffset];
                    }

                    if (decompressedSize != compressedSize)
                    {
                        DataFileDecrypter.UnpackData(outputBuffer, decompressedSize, indexData, compressedSize, dataOffset);
                    }
                    else
                    {
                        for (long byteIndex = 0; byteIndex < decompressedSize; byteIndex += 1)
                        {
                            outputBuffer[byteIndex] = indexData[dataOffset + byteIndex];
                        }
                    }

                    return outputBuffer;
                }

                dataOffset += compressedSize;
            }

            return null;
        }

        private static sbyte[] StreamToSbyte(BinaryReader stream, int length)
        {
            List<sbyte> result = [];
            int byteIndex = 0;

            while (byteIndex < length)
            {
                sbyte ReadByte = stream.ReadSByte();

                if (ReadByte == -1)
                {
                    break;
                }

                result.Add(ReadByte);
                byteIndex += 1;
            }

            return result.ToArray();
        }
    }
}