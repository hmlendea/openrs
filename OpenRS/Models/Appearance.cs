namespace OpenRS.Models
{
    public sealed class Appearance
    {
        public int HairColour { get; set; }

        public int TopColour { get; set; }

        public int TrousersColour { get; set; }

        public int SkinColour { get; set; }

        public int Head { get; set; }

        public int Body { get; set; }

        public bool IsValid => true; // TODO: Implement this

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
