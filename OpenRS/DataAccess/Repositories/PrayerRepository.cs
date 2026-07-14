using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class PrayerRepository(string fileName)
        : AutoSavingJsonRepository<PrayerEntity>(fileName);
}
