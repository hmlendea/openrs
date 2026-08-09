namespace OpenRS.Models
{
    public sealed class Animation
    {
        public int V1Id { get; set; }

        public string Name { get; set; }

        public int CharacterColour { get; set; }

        public int GenderModel { get; set; }

        public bool HasAttackFrames { get; set; }

        public bool HasFemaleFrames { get; set; }

        public int SpriteIndex { get; set; }
    }
}
