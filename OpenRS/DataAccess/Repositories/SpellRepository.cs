using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class SpellRepository(string fileName)
        : AutoSavingJsonRepository<SpellEntity>(fileName);
}
