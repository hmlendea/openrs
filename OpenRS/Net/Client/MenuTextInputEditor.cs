using Microsoft.Xna.Framework.Input;

namespace OpenRS.Net.Client
{
    internal static class MenuTextInputEditor
    {
        private static readonly string AllowedInputCharacters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖabcdefghijklmnopqrstuvwxyzåäö0123456789!\"" +
            (char)243 +
            "$%^&*()-_=+[{]};:'@#~,<.>/?\\| ";

        internal static string RemoveLastCharacter(
            Keys key,
            string text,
            int currentLength)
        {
            if (key == Keys.Back && currentLength > 0)
            {
                return text[..(currentLength - 1)];
            }

            return text;
        }

        internal static bool IsSubmitted(Keys key, int currentLength)
            => key == Keys.Enter && currentLength > 0;

        internal static string AppendCharacter(
            char character,
            string text,
            int currentLength,
            int maximumLength)
        {
            if (currentLength >= maximumLength)
            {
                return text;
            }

            for (int characterIndex = 0;
                characterIndex < AllowedInputCharacters.Length;
                characterIndex += 1)
            {
                if (character == AllowedInputCharacters[characterIndex])
                {
                    text += character;
                }
            }

            return text;
        }
    }
}