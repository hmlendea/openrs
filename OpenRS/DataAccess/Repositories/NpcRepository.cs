using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class NpcRepository(string fileName)
        : AutoSavingJsonRepository<NpcEntity>(fileName);
}
