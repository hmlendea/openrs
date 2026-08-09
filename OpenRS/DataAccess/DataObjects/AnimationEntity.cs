using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class AnimationEntity : EntityBase
    {
        public int V1Id { get; set; }

        public string Name { get; set; }

        public int CharacterColour { get; set; }

        public int GenderModel { get; set; }

        public int HasAttackFrames { get; set; }

        public int HasFemaleFrames { get; set; }

        public int SpriteIndex { get; set; }
    }
}
