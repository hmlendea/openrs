using System;

namespace OpenRS.Models
{
    internal static class AppearanceValidator
    {
        private static readonly int[] ValidHeadSprites = [1, 4, 6, 7, 8];
        private static readonly int[] ValidBodySprites = [2, 5];

        private static int MaximumHairColour => 9;

        private static int MaximumTopColour => 14;

        private static int MaximumTrousersColour => 14;

        private static int MaximumSkinColour => 4;

        internal static bool IsValid(Appearance appearance)
            => Array.IndexOf(ValidHeadSprites, appearance.Head) >= 0 &&
                Array.IndexOf(ValidBodySprites, appearance.Body) >= 0 &&
                IsColourValid(appearance.HairColour, MaximumHairColour) &&
                IsColourValid(appearance.TopColour, MaximumTopColour) &&
                IsColourValid(
                    appearance.TrousersColour,
                    MaximumTrousersColour) &&
                IsColourValid(appearance.SkinColour, MaximumSkinColour);

        private static bool IsColourValid(int colour, int maximumColour)
            => colour >= 0 && colour <= maximumColour;
    }
}