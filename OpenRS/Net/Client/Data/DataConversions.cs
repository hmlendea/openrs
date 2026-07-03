using System;

namespace OpenRS.Net.Client.Data
{
    public static class DataConversions
    {
        static readonly char[] characters = {
            ' ', 'e', 't', 'a', 'o', 'i', 'h', 'n', 's', 'r',
            'd', 'l', 'u', 'm', 'w', 'c', 'y', 'f', 'g', 'p',
            'b', 'v', 'k', 'x', 'j', 'q', 'z', '0', '1', '2',
            '3', '4', '5', '6', '7', '8', '9', ' ', '!', '?',
            '.', ',', ':', ';', '(', ')', '-', '&', '*', '\\',
            '\'', '@', '#', '+', '=', 'ó', '$', '%', '"', '[',
            ']'
        };

        public static string byteToString(sbyte[] data, int offset, int length)
        {
            char[] buffer = new char[100];

            try
            {
                int k = 0;
                int l = -1;

                for (int i1 = 0; i1 < length; i1++)
                {
                    int j1 = data[offset++] & 0xff;
                    int k1 = j1 >> 4 & 0xf;

                    if (l == -1)
                    {
                        if (k1 < 13)
                        {
                            buffer[k++] = characters[k1];
                        }
                        else
                        {
                            l = k1;
                        }
                    }
                    else
                    {
                        buffer[k++] = characters[((l << 4) + k1) - 195];
                        l = -1;
                    }

                    k1 = j1 & 0xf;

                    if (l == -1)
                    {
                        if (k1 < 13)
                        {
                            buffer[k++] = characters[k1];
                        }
                        else
                        {
                            l = k1;
                        }
                    }
                    else
                    {
                        buffer[k++] = characters[((l << 4) + k1) - 195];
                        l = -1;
                    }
                }

                bool flag = true;

                for (int l1 = 0; l1 < k; l1++)
                {
                    char c = buffer[l1];

                    if (l1 > 4 && c == '@')
                    {
                        buffer[l1] = ' ';
                    }

                    if (c == '%')
                    {
                        buffer[l1] = ' ';
                    }

                    if (flag && c >= 'a' && c <= 'z')
                    {
                        buffer[l1] += '\uFFE0';
                        flag = false;
                    }

                    if (c == '.' || c == '!' || c == ':')
                    {
                        flag = true;
                    }
                }

                return new string(buffer, 0, k);
            }
            catch (Exception)
            {
                Console.WriteLine($"An error has occured in {nameof(DataConversions)}.cs");
                return ".";
            }
        }
    }
}
