using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class WorldObjectRepository(string fileName)
        : AutoSavingJsonRepository<WorldObjectEntity>(fileName);
}
