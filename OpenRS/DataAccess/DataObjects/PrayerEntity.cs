using NuciDAL.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    public sealed class PrayerEntity : EntityBase
    {
        public int V1Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int RequiredLevel { get; set; }

        public int DrainRate { get; set; }
    }
}
