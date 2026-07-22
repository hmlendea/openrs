using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class ElevationRepository(string fileName)
        : AutoSavingJsonRepository<ElevationEntity>(fileName);
}

