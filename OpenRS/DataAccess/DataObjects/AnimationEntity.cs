using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class AnimationEntity : EntityBase
    {
        public string Name { get; set; }
        public int CharacterColour { get; set; }
        public int GenderModel { get; set; }

        public int HasA { get; set; }

        public int HasF { get; set; }

        public int Number { get; set; }
    }
}
