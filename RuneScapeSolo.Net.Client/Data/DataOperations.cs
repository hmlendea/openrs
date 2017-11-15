using System.IO;
using System;
using System.Net;
using System.Collections.Generic;

namespace RuneScapeSolo.Net.Client.Data
{
    public class DataOperations
    {
        public static MemoryStream openInputStream(string arg0)
        {

            //return org.moparscape.msc.client.DataOperations.getByte(byte0);

            Stream obj;
            if (codeBase == null)
            {
                obj = File.OpenRead(arg0);
            }
            else
            {
                Uri url = new Uri(codeBase, arg0);

                var req = HttpWebRequest.Create(url.ToString());

                obj = req.GetResponse().GetResponseStream();
            }

            MemoryStream memory = new MemoryStream();
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

        static sbyte[] streamToSbyte(BinaryReader stream, int length)
        {
            List<sbyte> list = new List<sbyte>();
            {
                sbyte ch;
                var i = 0;
                while ((ch = stream.ReadSByte()) != -1 && i < length)
                {
                    list.Add(ch);
                    i++;
                }
            }
            return list.ToArray();
        }

        public static void readFully(string p, sbyte[] abyte1, int i)
        {
            //org.moparscape.msc.client.DataOperations.readFully(p, abyte1, i);
            using (var stream = openInputStream(p))
            {
                abyte1 = streamToSbyte(new BinaryReader(stream), i);
                //for (int j = 0; j < i; j++) { 
                //    abyte1[j] = 
                //}
                //stream.Read(abyte1, 0, i);
            }
        }

        public static void readFully(string p, byte[] abyte1, int i)
        {
            using (var stream = openInputStream(p))
            {
                stream.Read(abyte1, 0, i);
            }
        }
        
        public static long GetLong(sbyte[] data, int i)
        {
            return ((GetInt32(data, i) & 0xffffffffL) << 32) +
                    (GetInt32(data, i + 4) & 0xffffffffL);
        }

        public static long GetLong(byte[] abyte0, int i)
        {
            return ((GetInt32(abyte0, i) & 0xffffffffL) << 32) +
                    (GetInt32(abyte0, i + 4) & 0xffffffffL);
        }


        public static int getShort2(sbyte[] abyte0, int i)
        {
            int j = GetInt8(abyte0[i]) * 256 + GetInt8(abyte0[i + 1]);
            if (j > 32767)
            {
                j -= 0x10000;
            }
            return j;
        }

        public static int GetInt8(sbyte value)
        {
            return value & 255;
        }

        public static int GetInt16(sbyte[] data, int index)
        {
            return ((data[index] & 255) << 8) +
                    (data[index + 1] & 255);
        }

        public static int GetInt32(sbyte[] data, int index)
        {
            return ((data[index] & 255) << 24) +
                   ((data[index + 1] & 255) << 16) +
                   ((data[index + 2] & 255) << 8) +
                    (data[index + 3] & 255);
        }

        public static int GetInt32(byte[] data, int index)
        {
            return ((data[index] & 255) << 24) +
                   ((data[index + 1] & 255) << 16) +
                   ((data[index + 2] & 255) << 8) +
                    (data[index + 3] & 255);
        }

        public static int GetInt(sbyte[] data, int offset, int length)
        {
            int bitOffset = offset >> 3;
            int bitMod = 8 - (offset & 7);

            int value = 0;

            for (; length > bitMod; bitMod = 8)
            {
                value += (data[bitOffset++] & bitMask[bitMod]) << length - bitMod;
                length -= bitMod;
            }

            if (length == bitMod)
            {
                value += data[bitOffset] & bitMask[bitMod];
            }
            else
            {
                value += data[bitOffset] >> bitMod - length & bitMask[length];
            }

            return value;
        }
        
        public static string FormatString(string str, int length)
        {
            string s = "";

            for (int i = 0; i < length; i++)
            {
                if (i >= str.Length)
                {
                    s = s + " ";
                    continue;
                }

                if (!char.IsLetterOrDigit(str[i]))
                {
                    s = s + '_';
                    continue;
                }

                s = s + str[i];
            }

            return s;
        }

        public static string IpToString(int ip)
        {
            return (ip >> 24 & 255) + "." +
                   (ip >> 16 & 255) + "." +
                   (ip >> 8 & 255) + "." +
                   (ip & 255);
        }

        public static long nameToHash(string arg0)
        {
            string s = "";
            for (int i = 0; i < arg0.Length; i++)
            {
                char c = arg0[i];
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

        public static string LongToString(long hash)
        {
            if (hash < 0L)
            {
                return "invalid_name";
            }

            string str = "";

            while (hash != 0L)
            {
                int i = (int)(hash % 37L);
                hash /= 37L;

                if (i == 0)
                {
                    str = " " + str;
                }
                else
                {
                    if (i < 27)
                    {
                        if (hash % 37L == 0L)
                        {
                            str = (char)((i + 65) - 1) + str;
                        }
                        else
                        {
                            str = (char)((i + 97) - 1) + str;
                        }
                    }
                    else
                    {
                        str = (char)((i + 48) - 27) + str;
                    }
                }
            }

            return str;
        }

        public static long getObjectOffset(string objName, sbyte[] objData)
        {
            //return org.moparscape.msc.client.DataOperations.getObjectOffset(objName, objData);

            int i = GetInt16(objData, 0);
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
        
        public static byte[] loadData(string s, int i, byte[] abyte0)
        {
            return loadData(s, i, abyte0, null);
        }

        public static byte[] loadData(string arg0, int arg1, byte[] arg2, byte[] arg3)
        {
            //return org.moparscape.msc.client.DataOperations.loadData(objName, objData, arg2, arg3);

            int i = (arg2[0] & 0xff) * 256 + (arg2[1] & 0xff);
            int j = 0;
            arg0 = arg0.ToUpper();

            for (int k = 0; k < arg0.Length; k++)
            {
                j = (j * 61 + arg0[k]) - 32;
            }

            int l = 2 + i * 10;

            for (int i1 = 0; i1 < i; i1++)
            {
                int j1 = (arg2[i1 * 10 + 2] & 0xff) * 0x1000000 + (arg2[i1 * 10 + 3] & 0xff) * 0x10000 + (arg2[i1 * 10 + 4] & 0xff) * 256 + (arg2[i1 * 10 + 5] & 0xff);
                int k1 = (arg2[i1 * 10 + 6] & 0xff) * 0x10000 + (arg2[i1 * 10 + 7] & 0xff) * 256 + (arg2[i1 * 10 + 8] & 0xff);
                int l1 = (arg2[i1 * 10 + 9] & 0xff) * 0x10000 + (arg2[i1 * 10 + 10] & 0xff) * 256 + (arg2[i1 * 10 + 11] & 0xff);

                if (j1 == j)
                {
                    if (arg3 == null)
                    {
                        arg3 = new byte[k1 + arg1];
                    }

                    if (k1 != l1)
                    {
                        DataFileDecrypter.unpackData(arg3, k1, arg2, l1, l);
                    }
                    else
                    {
                        for (long i2 = 0; i2 < k1; i2++)
                        {
                            arg3[i2] = arg2[l + i2];
                        }

                    }

                    return arg3;
                }

                l += l1;
            }

            return null;
        }

        public static sbyte[] loadData(string s, int i, sbyte[] abyte0)
        {
            return loadData(s, i, abyte0, null);
        }

        public static sbyte[] loadData(string arg0, int arg1, sbyte[] arg2, sbyte[] arg3)
        {
            //return org.moparscape.msc.client.DataOperations.loadData(objName, objData, arg2, arg3);

            int i = (arg2[0] & 0xff) * 256 + (arg2[1] & 0xff);
            int j = 0;
            arg0 = arg0.ToUpper();
            for (int k = 0; k < arg0.Length; k++)
            {
                j = (j * 61 + arg0[k]) - 32;
            }

            int l = 2 + i * 10;
            for (int i1 = 0; i1 < i; i1++)
            {
                int j1 = (arg2[i1 * 10 + 2] & 0xff) * 0x1000000 + (arg2[i1 * 10 + 3] & 0xff) * 0x10000 + (arg2[i1 * 10 + 4] & 0xff) * 256 + (arg2[i1 * 10 + 5] & 0xff);
                int k1 = (arg2[i1 * 10 + 6] & 0xff) * 0x10000 + (arg2[i1 * 10 + 7] & 0xff) * 256 + (arg2[i1 * 10 + 8] & 0xff);
                int l1 = (arg2[i1 * 10 + 9] & 0xff) * 0x10000 + (arg2[i1 * 10 + 10] & 0xff) * 256 + (arg2[i1 * 10 + 11] & 0xff);
                if (j1 == j)
                {
                    if (arg3 == null)
                    {
                        arg3 = new sbyte[k1 + arg1];
                    }
                    if (k1 != l1)
                    {
                        DataFileDecrypter.unpackData(arg3, k1, arg2, l1, l);
                    }
                    else
                    {
                        for (long i2 = 0; i2 < k1; i2++)
                        {
                            arg3[i2] = arg2[l + i2];
                        }

                    }
                    return arg3;
                }
                l += l1;
            }

            return null;
        }

        public static Uri codeBase = null;
        static int[] bitMask = { 0, 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023, 2047, 4095, 8191,
            16383, 32767, 65535, 0x1ffff, 0x3ffff, 0x7ffff, 0xfffff, 0x1fffff, 0x3fffff, 0x7fffff,
            0xffffff, 0x1ffffff, 0x3ffffff, 0x7ffffff, 0xfffffff, 0x1fffffff, 0x3fffffff, 0x7fffffff, -1 };
    }
}