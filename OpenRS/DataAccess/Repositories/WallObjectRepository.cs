using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class WallObjectRepository(string fileName)
        : AutoSavingJsonRepository<WallObjectEntity>(fileName);
}
