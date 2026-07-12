using System.IO;
using System;
using System.Net;
using System.Collections.Generic;
using OpenRS.Settings;
namespace OpenRS.Net.Client.Data
{


    public sealed class DataOperations
    {

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        //ORIGINAL LINE: public static java.io.InputStream openInputStream(String objName) throws java.io.IOException
        //public static InputStream openInputStream(string objName)
        //{
        //    object obj;
        //    if (codeBase is null)
        //    {
        //        obj = new BufferedInputStream(new FileInputStream(objName));
        //    }
        //    else
        //    {
        //        URL url = new URL(codeBase, objName);
        //        obj = url.openStream();
        //    }
        //    return ((InputStream)(obj));
        //}

        public static MemoryStream openInputStream(String fileName)
        {

            //return org.moparscape.msc.client.DataOperations.getByte(byte0);

            Stream obj;
            if (codeBase is null)
            {
                obj = File.OpenRead(Path.Combine(Config.ConfigurationDirectory, fileName));
            }
            else
            {
                Uri url = new(codeBase, fileName);

                WebRequest webRequest = HttpWebRequest.Create(url.ToString());

                obj = webRequest.GetResponse().GetResponseStream();
            }

            MemoryStream memory = new();
            int j = 0;
            byte[] buffer = new byte[2048];



            //using (BinaryWriter binaryWriter = new BinaryWriter(outputStream))
            //{
            //    binaryWriter.Write((sbyte[])(Array)sbytes);
            //}


            while ((j = obj.Read(buffer, 0, buffer.Length)) != 0)
            {
                //memory.Write(buffer, 0, j);
                memory.Write((byte[])(Array)buffer, 0, j);
            }


            return memory;
        }

        static private sbyte[] streamToSbyte(BinaryReader stream, int length)
        {
            List<sbyte> list = [];
            {
                sbyte ch;
                int byteIndex = 0;
                while ((ch = stream.ReadSByte()) != -1 && byteIndex < length)
                {
                    list.Add(ch);
                    byteIndex += 1;
                }
            }
            return list.ToArray();
        }

        public static void readFully(string p, sbyte[] abyte1, int i)
        {
            //org.moparscape.msc.client.DataOperations.readFully(p, abyte1, i);
            using MemoryStream stream = openInputStream(p);
            abyte1 = streamToSbyte(new BinaryReader(stream), i);
            //for (int j = 0; j < i; j++) {
            //    abyte1[j] =
            //}
            //stream.Read(abyte1, 0, i);
        }

        public static void readFully(string p, byte[] abyte1, int i)
        {
            using MemoryStream stream = openInputStream(p);
            stream.Read(abyte1, 0, i);
        }

        ////JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
        ////ORIGINAL LINE: public static void readFully(String s, byte abyte0[] , int i) throws java.io.IOException
        //public static void readFully(string s, sbyte[] abyte0, int i)
        //{
        //    InputStream inputstream = openInputStream(s);
        //    DataInputStream datainputstream = new DataInputStream(inputstream);
        //    try
        //    {
        //        datainputstream.readFully(abyte0, 0, i);
        //    }
        //    catch (EOFException _ex)
        //    {
        //    }
        //    datainputstream.close();
        //}


        //public static int getByte(byte byte0)
        //{
        //    //return org.moparscape.msc.client.DataOperations.getByte(byte0);
        //    return byte0 & 0xff;
        //}
        public static int getByte(sbyte byte0)
        {
            return byte0 & 0xff;
        }
        //public static int getShort(byte[] abyte0, int i)
        //{
        //    //return org.moparscape.msc.client.DataOperations.getShort(abyte0, i);
        //    var val = ((abyte0[i] & 0xff) << 8) + (abyte0[i + 1] & 0xff);
        //    var val2 = abyte0[i];
        //    var val3 = abyte0[i] & 0xff;
        //    var val4 = abyte0[i] & 0xff;
        //    var val5 = abyte0[i + 1] & 0xff;
        //    return val;
        //}


        public static int getShort(sbyte[] abyte0, int i)
        {
            //return org.moparscape.msc.client.DataOperations.getShort(abyte0, i);
            int shortValue = ((abyte0[i] & 0xff) << 8) + (abyte0[i + 1] & 0xff);
            int highByte = abyte0[i];
            int highByteUnsigned = abyte0[i] & 0xff;
            int highByteUnsigned2 = abyte0[i] & 0xff;
            int lowByteUnsigned = abyte0[i + 1] & 0xff;
            return shortValue;
        }

        public static int GetInt16(sbyte[] abyte0, int i) => getShort(abyte0, i);

        //public static int getInt(sbyte[] abyte0, int i)
        //{
        //    return ((abyte0[i] & 0xff) << 24) + ((abyte0[i + 1] & 0xff) << 16) + ((abyte0[i + 2] & 0xff) << 8) + (abyte0[i + 3] & 0xff);
        //}

        public static long getLong(sbyte[] abyte0, int i)
        {
            //return org.moparscape.msc.client.DataOperations.getLong(abyte0, i);
            return (((long)getInt(abyte0, i) & 0xffffffffL) << 32) + ((long)getInt(abyte0, i + 4) & 0xffffffffL);
        }

        public static long getLong(byte[] abyte0, int i)
        {
            //return org.moparscape.msc.client.DataOperations.getLong(abyte0, i);
            return (((long)getInt(abyte0, i) & 0xffffffffL) << 32) + ((long)getInt(abyte0, i + 4) & 0xffffffffL);
        }

        //public static int getShort(sbyte[] abyte0, int i)
        //{
        //    return ((abyte0[i] & 0xff) << 8) + (abyte0[i + 1] & 0xff);
        //}

        public static int getInt(sbyte[] abyte0, int i)
        {
            //return org.moparscape.msc.client.DataOperations.getInt(abyte0, i);
            return ((abyte0[i] & 0xff) << 24) + ((abyte0[i + 1] & 0xff) << 16) + ((abyte0[i + 2] & 0xff) << 8) + (abyte0[i + 3] & 0xff);
        }

        public static int getInt(byte[] abyte0, int i)
        {
            //return org.moparscape.msc.client.DataOperations.getInt(abyte0, i);
            return ((abyte0[i] & 0xff) << 24) + ((abyte0[i + 1] & 0xff) << 16) + ((abyte0[i + 2] & 0xff) << 8) + (abyte0[i + 3] & 0xff);
        }

        //public static long getLong(sbyte[] abyte0, int i)
        //{
        //    return (((long)getInt(abyte0, i) & 0xffffffffL) << 32) + ((long)getInt(abyte0, i + 4) & 0xffffffffL);
        //}
        public static int getShort2(sbyte[] abyte0, int i)
        {
            int j = getByte(abyte0[i]) * 256 + getByte(abyte0[i + 1]);
            if (j > 32767)
            {
                j -= 0x10000;
            }
            return j;
        }

        //public static int getShort2(byte[] abyte0, int i)
        //{
        //    int j = getByte(abyte0[i]) * 256 + getByte(abyte0[i + 1]);
        //    if (j > 32767)
        //    {
        //        j -= 0x10000;
        //    }
        //    return j;
        //}


        public static int getBits(sbyte[] bytes, int off, int len)
        {
            //return org.moparscape.msc.client.DataOperations.getBits(bytes, off, len);

            int bitOff = off >> 3;
            int bitMod = 8 - (off & 7);
            int k = 0;
            for (; len > bitMod; bitMod = 8)
            {
                k += (bytes[bitOff++] & bitMask[bitMod]) << len - bitMod;
                len -= bitMod;
            }

            if (len == bitMod)
            {
                k += bytes[bitOff] & bitMask[bitMod];
            }
            else
            {
                k += bytes[bitOff] >> bitMod - len & bitMask[len];
            }
            return k;
        }

        //public static int getBits(byte[] bytes, int off, int len)
        //{
        //    int bitOff = off >> 3;
        //    int bitMod = 8 - (off & 7);
        //    int k = 0;
        //    for (; len > bitMod; bitMod = 8)
        //    {
        //        k += (bytes[bitOff++] & bitMask[bitMod]) << len - bitMod;
        //        len -= bitMod;
        //    }

        //    if (len == bitMod)
        //    {
        //        k += bytes[bitOff] & bitMask[bitMod];
        //    }
        //    else
        //    {
        //        k += bytes[bitOff] >> bitMod - len & bitMask[len];
        //    }
        //    return k;
        //}

        public static string formatString(string text, int maxLength)
        {
            string s = "";
            for (int i = 0; i < maxLength; i++)
            {
                if (i >= text.Length)
                {
                    s = s + " ";
                }
                else
                {
                    char c = text[i];
                    if (c >= 'a' && c <= 'z')
                    {
                        s = s + c;
                    }
                    else
                    {
                        if (c >= 'A' && c <= 'Z')
                        {
                            s = s + c;
                        }
                        else
                        {
                            if (c >= '0' && c <= '9')
                            {
                                s = s + c;
                            }
                            else
                            {
                                s = s + '_';
                            }
                        }
                    }
                }
            }

            return s;
        }

        public static string ipToString(int i)
        {
            return (i >> 24 & 0xff) + "." + (i >> 16 & 0xff) + "." + (i >> 8 & 0xff) + "." + (i & 0xff);
        }

        public static long nameToHash(string name)
        {
            string s = "";
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (c >= 'a' && c <= 'z')
                {
                    s = s + c;
                }
                else
                {
                    if (c >= 'A' && c <= 'Z')
                    {
                        s = s + (char)((c + 97) - 65);
                    }
                    else
                    {
                        if (c >= '0' && c <= '9')
                        {
                            s = s + c;
                        }
                        else
                        {
                            s = s + ' ';
                        }
                    }
                }
            }

            s = s.Trim();
            if (s.Length > 12)
            {
                s = s.Substring(0, 12);
            }
            long l = 0L;
            for (int j = 0; j < s.Length; j++)
            {
                char c1 = s[j];
                l *= 37L;
                if (c1 >= 'a' && c1 <= 'z')
                {
                    l += (1 + c1) - 97;
                }
                else
                {
                    if (c1 >= '0' && c1 <= '9')
                    {
                        l += (27 + c1) - 48;
                    }
                }
            }

            return l;
        }

        public static string hashToName(long hash)
        {
            if (hash < 0L)
            {
                return "invalid_name";
            }
            string s = "";
            while (!hash.Equals(0L))
            {
                int i = (int)(hash % 37L);
                hash /= 37L;
                if (i == 0)
                {
                    s = " " + s;
                }
                else
                {
                    if (i < 27)
                    {
                        if (hash % 37L == 0L)
                        {
                            s = (char)((i + 65) - 1) + s;
                        }
                        else
                        {
                            s = (char)((i + 97) - 1) + s;
                        }
                    }
                    else
                    {
                        s = (char)((i + 48) - 27) + s;
                    }
                }
            }
            return s;
        }

        public static long getObjectOffset(string objName, sbyte[] objData)
        {
            //return org.moparscape.msc.client.DataOperations.getObjectOffset(objName, objData);

            int i = getShort(objData, 0);
            int j = 0;
            objName = objName.ToUpper();
            for (int k = 0; k < objName.Length; k++)
            {
                j = (j * 61 + objName[k]) - 32;
            }

            long l = 2 + i * 10;
            for (int i1 = 0; i1 < i; i1++)
            {
                long j1 = (objData[i1 * 10 + 2] & 0xff) * 0x1000000 + (objData[i1 * 10 + 3] & 0xff) * 0x10000 + (objData[i1 * 10 + 4] & 0xff) * 256 + (objData[i1 * 10 + 5] & 0xff);
                long k1 = (objData[i1 * 10 + 9] & 0xff) * 0x10000 + (objData[i1 * 10 + 10] & 0xff) * 256 + (objData[i1 * 10 + 11] & 0xff);
                if (j1 == j)
                {
                    return l;
                }
                l += k1;
            }

            return 0;
        }

        public static int getSoundLength(string soundName, sbyte[] soundIndex)
        {
            // return org.moparscape.msc.client.DataOperations.getSoundLength(objName, objData);

            int i = getShort(soundIndex, 0);
            int j = 0;
            soundName = soundName.ToUpper();
            for (int k = 0; k < soundName.Length; k++)
            {
                j = (j * 61 + soundName[k]) - 32;
            }

            for (int i1 = 0; i1 < i; i1++)
            {
                int j1 = (soundIndex[i1 * 10 + 2] & 0xff) * 0x1000000 + (soundIndex[i1 * 10 + 3] & 0xff) * 0x10000 + (soundIndex[i1 * 10 + 4] & 0xff) * 256 + (soundIndex[i1 * 10 + 5] & 0xff);
                int k1 = (soundIndex[i1 * 10 + 6] & 0xff) * 0x10000 + (soundIndex[i1 * 10 + 7] & 0xff) * 256 + (soundIndex[i1 * 10 + 8] & 0xff);
                if (j1 == j)
                {
                    return k1;
                }
            }

            return 0;
        }

        public static byte[] loadData(string s, int i, byte[] abyte0)
        {
            return loadData(s, i, abyte0, null);
        }

        public static byte[] loadData(string entryName, int outputOffset, byte[] indexData, byte[] outputBuffer)
        {

            //return org.moparscape.msc.client.DataOperations.loadData(objName, objData, arg2, arg3);

            int i = (indexData[0] & 0xff) * 256 + (indexData[1] & 0xff);
            int j = 0;
            entryName = entryName.ToUpper();
            for (int k = 0; k < entryName.Length; k++)
            {
                j = (j * 61 + entryName[k]) - 32;
            }

            int l = 2 + i * 10;
            for (int i1 = 0; i1 < i; i1++)
            {
                int j1 = (indexData[i1 * 10 + 2] & 0xff) * 0x1000000 + (indexData[i1 * 10 + 3] & 0xff) * 0x10000 + (indexData[i1 * 10 + 4] & 0xff) * 256 + (indexData[i1 * 10 + 5] & 0xff);
                int k1 = (indexData[i1 * 10 + 6] & 0xff) * 0x10000 + (indexData[i1 * 10 + 7] & 0xff) * 256 + (indexData[i1 * 10 + 8] & 0xff);
                int l1 = (indexData[i1 * 10 + 9] & 0xff) * 0x10000 + (indexData[i1 * 10 + 10] & 0xff) * 256 + (indexData[i1 * 10 + 11] & 0xff);
                if (j1.Equals(j))
                {
                    if (outputBuffer is null)
                    {
                        outputBuffer = new byte[k1 + outputOffset];
                    }
                    if (k1 != l1)
                    {
                        DataFileDecrypter.unpackData(outputBuffer, k1, indexData, l1, l);
                    }
                    else
                    {
                        for (long i2 = 0; i2 < k1; i2++)
                        {
                            outputBuffer[i2] = indexData[l + i2];
                        }

                    }
                    return outputBuffer;
                }
                l += l1;
            }

            return null;
        }

        public static sbyte[] loadData(string s, int i, sbyte[] abyte0)
        {
            return loadData(s, i, abyte0, null);
        }

        public static sbyte[] loadData(string entryNameS, int outputOffsetS, sbyte[] indexDataS, sbyte[] outputBufferS)
        {

            //return org.moparscape.msc.client.DataOperations.loadData(objName, objData, arg2, arg3);

            int i = (indexDataS[0] & 0xff) * 256 + (indexDataS[1] & 0xff);
            int j = 0;
            entryNameS = entryNameS.ToUpper();
            for (int k = 0; k < entryNameS.Length; k++)
            {
                j = (j * 61 + entryNameS[k]) - 32;
            }

            int l = 2 + i * 10;
            for (int i1 = 0; i1 < i; i1++)
            {
                int j1 = (indexDataS[i1 * 10 + 2] & 0xff) * 0x1000000 + (indexDataS[i1 * 10 + 3] & 0xff) * 0x10000 + (indexDataS[i1 * 10 + 4] & 0xff) * 256 + (indexDataS[i1 * 10 + 5] & 0xff);
                int k1 = (indexDataS[i1 * 10 + 6] & 0xff) * 0x10000 + (indexDataS[i1 * 10 + 7] & 0xff) * 256 + (indexDataS[i1 * 10 + 8] & 0xff);
                int l1 = (indexDataS[i1 * 10 + 9] & 0xff) * 0x10000 + (indexDataS[i1 * 10 + 10] & 0xff) * 256 + (indexDataS[i1 * 10 + 11] & 0xff);
                if (j1.Equals(j))
                {
                    if (outputBufferS is null)
                    {
                        outputBufferS = new sbyte[k1 + outputOffsetS];
                    }
                    if (k1 != l1)
                    {
                        DataFileDecrypter.unpackData(outputBufferS, k1, indexDataS, l1, l);
                    }
                    else
                    {
                        for (long i2 = 0; i2 < k1; i2++)
                        {
                            outputBufferS[i2] = indexDataS[l + i2];
                        }

                    }
                    return outputBufferS;
                }
                l += l1;
            }

            return null;
        }

        public static Uri codeBase = null;
        private static int[] bitMask = [0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191, 16383, 32767, 65535, 0x1ffff, 0x3ffff, 0x7ffff, 0xfffff, 0x1fffff, 0x3fffff, 0x7fffff, 0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff, 0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff, -1];

    }

}