namespace OpenRS.Models
{
    public sealed class Appearance
    {
        private static int AppearanceSpriteCount => 12;
        private static int DefaultLegsSprite => 3;
        private static int NoSprite => 0;

        public int HairColour { get; set; }

        public int TopColour { get; set; }

        public int TrousersColour { get; set; }

        public int SkinColour { get; set; }

        public int Head { get; set; }

        public int Body { get; set; }

        public bool IsValid => AppearanceValidator.IsValid(this);

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
    }
}
