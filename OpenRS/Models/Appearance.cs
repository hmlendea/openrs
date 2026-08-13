using System;

namespace OpenRS.Models
{
    public sealed class Appearance
    {
        private static readonly int[] ValidHeadSprites = [1, 4, 6, 7, 8];
        private static readonly int[] ValidBodySprites = [2, 5];

        private static int AppearanceSpriteCount => 12;
        private static int DefaultLegsSprite => 3;
        private static int MaximumHairColour => 9;
        private static int MaximumTopColour => 14;
        private static int MaximumTrousersColour => 14;
        private static int MaximumSkinColour => 4;
        private static int NoSprite => 0;

        public int HairColour { get; set; }

        public int TopColour { get; set; }

        public int TrousersColour { get; set; }

        public int SkinColour { get; set; }

        public int Head { get; set; }

        public int Body { get; set; }

        public bool IsValid =>
            Array.IndexOf(ValidHeadSprites, Head) >= 0 &&
            Array.IndexOf(ValidBodySprites, Body) >= 0 &&
            IsColourValid(HairColour, MaximumHairColour) &&
            IsColourValid(TopColour, MaximumTopColour) &&
            IsColourValid(TrousersColour, MaximumTrousersColour) &&
            IsColourValid(SkinColour, MaximumSkinColour);

        public int GetSprite(int position) => position switch
        {
            0 => Head,
            1 => Body,
            2 => DefaultLegsSprite,
            _ => NoSprite,
        };

        public int[] GetSprites()
        {
            int[] sprites = new int[AppearanceSpriteCount];
            sprites[0] = Head;
            sprites[1] = Body;
            sprites[2] = DefaultLegsSprite;

            return sprites;
        }

        private static bool IsColourValid(int colour, int maximumColour)
            => colour >= 0 && colour <= maximumColour;
    }
}
