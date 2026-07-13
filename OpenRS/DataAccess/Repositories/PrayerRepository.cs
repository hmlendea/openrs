using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class PrayerRepository(string fileName) : XmlRepository<PrayerEntity>(fileName)
    {
        public override void Update(PrayerEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
