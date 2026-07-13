using System;

namespace OpenRS.Net.Client.Game
{

public sealed class ChatMessage {

    public static string BytesToString(sbyte[] encodedBytes, int readOffset, int byteCount)
    {
        try {
            int i = 0;
            int j = -1;
            for(int k = 0; k < byteCount; k += 1) {
                int l = encodedBytes[readOffset += 1] & 0xff;
                int i1 = l >> 4 & 0xf;
                if(j == -1) {
                    if(i1 < 13)
                        {
                            chatMessage[i++] = validChars[i1];
                        }
                        else
                        {
                            j = i1;
                        }
                    } else {
                    chatMessage[i++] = validChars[(j << 4) + i1 - 195];
                    j = -1;
                }
                i1 = l & 0xf;
                if(j == -1) {
                    if(i1 < 13)
                        {
                            chatMessage[i++] = validChars[i1];
                        }
                        else
                        {
                            j = i1;
                        }
                    } else {
                    chatMessage[i++] = validChars[(j << 4) + i1 - 195];
                    j = -1;
                }
            }

            bool flag = true;
            for(int j1 = 0; j1 < i; j1 += 1) {
                char c = chatMessage[j1];
                if(j1 > 4 && c == '@')
                    {
                        chatMessage[j1] = ' ';
                    }

                    if (c == '%')
                    {
                        chatMessage[j1] = ' ';
                    }

                    if (flag && c >= 'a' && c <= 'z') {
                    chatMessage[j1] += '\uFFE0';
                    flag = false;
                }
                if(c == '.' || c == '!')
                    {
                        flag = true;
                    }
                }

            return new string(chatMessage, 0, i);
        }
        catch(Exception _ex) {
            return ".";
        }
    }

    public static string BytesToString(byte[] encodedBytes, int readOffset, int byteCount)
    {
        try
        {
            int i = 0;
            int j = -1;
            for (int k = 0; k < byteCount; k += 1)
            {
                int l = encodedBytes[readOffset += 1] & 0xff;
                int i1 = l >> 4 & 0xf;
                if (j == -1)
                {
                    if (i1 < 13)
                        {
                            chatMessage[i++] = validChars[i1];
                        }
                        else
                        {
                            j = i1;
                        }
                    }
                else
                {
                    chatMessage[i++] = validChars[(j << 4) + i1 - 195];
                    j = -1;
                }
                i1 = l & 0xf;
                if (j == -1)
                {
                    if (i1 < 13)
                        {
                            chatMessage[i++] = validChars[i1];
                        }
                        else
                        {
                            j = i1;
                        }
                    }
                else
                {
                    chatMessage[i++] = validChars[(j << 4) + i1 - 195];
                    j = -1;
                }
            }

            bool flag = true;
            for (int j1 = 0; j1 < i; j1 += 1)
            {
                char c = chatMessage[j1];
                if (j1 > 4 && c == '@')
                    {
                        chatMessage[j1] = ' ';
                    }

                    if (c == '%')
                    {
                        chatMessage[j1] = ' ';
                    }

                    if (flag && c >= 'a' && c <= 'z')
                {
                    chatMessage[j1] += '\uFFE0';
                    flag = false;
                }
                if (c == '.' || c == '!')
                    {
                        flag = true;
                    }
                }

            return new string(chatMessage, 0, i);
        }
        catch (Exception _ex)
        {
            return ".";
        }
    }

    public static int StringToBytes(string message) {
        if(message.Length > 80)
            {
                message = message.Substring(0, 80);
            }

            message = message.ToLower();
        int i = 0;
        int j = -1;
        for(int k = 0; k < message.Length; k += 1) {
            char c = message[k];
            int l = 0;
            for(int i1 = 0; i1 < validChars.Length; i1 += 1) {
                if(c != validChars[i1])
                    {
                        continue;
                    }

                    l = i1;
                break;
            }

            if(l > 12)
                {
                    l += 195;
                }

                if (j == -1) {
                if(l < 13)
                    {
                        j = l;
                    }
                    else
                    {
                        lastChat[i++] = (byte)l;
                    }
                } else
            if(l < 13) {
                lastChat[i++] = (byte)((j << 4) + l);
                j = -1;
            } else {
                lastChat[i++] = (byte)((j << 4) + (l >> 4));
                j = l & 0xf;
            }
        }

        if(j != -1)
            {
                lastChat[i++] = (byte)(j << 4);
            }

            return i;
    }

    public static byte[] lastChat = new byte[100];
    public static char[] chatMessage = new char[100];
    private static readonly char[] validChars = [
        ' ', 'e', 't', 'a', 'o', 'i', 'h', 'n', 's', 'r',
        'd', 'l', 'u', 'm', 'w', 'c', 'y', 'f', 'g', 'p',
        'b', 'v', 'k', 'x', 'j', 'q', 'z', '0', '1', '2',
        '3', '4', '5', '6', '7', '8', '9', ' ', '!', '?',
        '.', ',', ':', ';', '(', ')', '-', '&', '*', '\\',
        '\'', '@', '#', '+', '=', '§', '$', '%', '"', '[',
        ']'
    ];

}

}
