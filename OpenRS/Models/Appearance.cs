namespace OpenRS.Models
{
    public sealed class Appearance
    {
        private static readonly int[] ValidHeadSprites = [1, 4, 6, 7, 8];
        private static readonly int[] ValidBodySprites = [2, 5];

        private static int MaximumHairColour => 9;
        private static int MaximumTopColour => 14;
        private static int MaximumTrousersColour => 14;
        private static int MaximumSkinColour => 4;

        public int HairColour { get; set; }

        public int TopColour { get; set; }

        public int TrousersColour { get; set; }

        public int SkinColour { get; set; }

        public int Head { get; set; }

        public int Body { get; set; }

        public bool IsValid =>
            System.Array.IndexOf(ValidHeadSprites, Head) >= 0 &&
            System.Array.IndexOf(ValidBodySprites, Body) >= 0 &&
            HairColour >= 0 && HairColour <= MaximumHairColour &&
            TopColour >= 0 && TopColour <= MaximumTopColour &&
            TrousersColour >= 0 && TrousersColour <= MaximumTrousersColour &&
            SkinColour >= 0 && SkinColour <= MaximumSkinColour;

        public int GetSprite(int position) => position switch
        {
            0 => Head,
            1 => Body,
            2 => 3,
            _ => 0,
        };

        public int[] GetSprites() => [Head, Body, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0];
    }
}
