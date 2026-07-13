namespace OpenRS.Models
{
    public sealed class Animation
    {
        public string Name { get; set; }
        public int CharacterColour { get; set; }
        public int GenderModel { get; set; }

        // TODO: Convert to bool.
        public int HasA { get; set; }

        // TODO: Convert to bool.
        public int HasF { get; set; }

        public int Number { get; set; }
    }
}
