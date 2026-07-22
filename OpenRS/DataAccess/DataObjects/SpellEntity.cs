using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class SpellEntity : EntityBase
    {
        public int V1Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int RequiredLevel { get; set; }

        public int Type { get; set; }

        public int RuneCount { get; set; }

        public int[] RequiredRunesIds { get; set; }

        public int[] RequiredRunesCounts { get; set; }

        public int ExperienceGain { get; set; }
    }
}
