using System;
using System.Text;

namespace OpenRS.Net.Client.Data
{
    internal sealed class PlayerNameEncoder
    {
        private static long NameHashBase => 37L;
        private static int MaxNameLength => 12;
        private static int NameHashDigitOffset => 27;
        private static string InvalidNameText => "invalid_name";

        internal static string FormatString(string text, int maxLength)
        {
            StringBuilder result = new();
            int characterLimit = Math.Min(maxLength, text.Length);

            for (int charIndex = 0; charIndex < characterLimit; charIndex += 1)
            {
                char character = text[charIndex];

                if ((character >= 'a' && character <= 'z') ||
                    (character >= 'A' && character <= 'Z') ||
                    (character >= '0' && character <= '9'))
                {
                    result.Append(character);
                }
                else
                {
                    result.Append('_');
                }
            }

            return result.ToString().PadRight(maxLength);
        }

        internal static long NameToHash(string name)
        {
            StringBuilder normalisedNameBuilder = new();

            foreach (char character in name)
            {
                if (character >= 'a' && character <= 'z')
                {
                    normalisedNameBuilder.Append(character);
                }
                else if (character >= 'A' && character <= 'Z')
                {
                    normalisedNameBuilder.Append((char)(character + 'a' - 'A'));
                }
                else if (character >= '0' && character <= '9')
                {
                    normalisedNameBuilder.Append(character);
                }
                else
                {
                    normalisedNameBuilder.Append(' ');
                }
            }

            string normalisedName = normalisedNameBuilder.ToString().Trim();

            if (normalisedName.Length > MaxNameLength)
            {
                normalisedName = normalisedName[..MaxNameLength];
            }

            long hashValue = 0L;

            foreach (char character in normalisedName)
            {
                hashValue *= NameHashBase;

                if (character >= 'a' && character <= 'z')
                {
                    hashValue += 1 + character - 'a';
                }
                else if (character >= '0' && character <= '9')
                {
                    hashValue += NameHashDigitOffset + character - '0';
                }
            }

            return hashValue;
        }

        internal static string HashToName(long hash)
        {
            if (hash < 0L)
            {
                return InvalidNameText;
            }

            StringBuilder nameBuilder = new();

            while (hash != 0L)
            {
                int remainder = (int)(hash % NameHashBase);
                hash /= NameHashBase;

                if (remainder == 0)
                {
                    nameBuilder.Append(' ');
                }
                else if (remainder < NameHashDigitOffset)
                {
                    if (hash % NameHashBase == 0L)
                    {
                        nameBuilder.Append((char)(remainder + 'A' - 1));
                    }
                    else
                    {
                        nameBuilder.Append((char)(remainder + 'a' - 1));
                    }
                }
                else
                {
                    nameBuilder.Append((char)(remainder + '0' - NameHashDigitOffset));
                }
            }

            char[] nameChars = nameBuilder.ToString().ToCharArray();
            Array.Reverse(nameChars);

            return new string(nameChars);
        }
    }
}

