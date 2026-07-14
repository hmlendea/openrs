using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class ItemRepository(string fileName)
        : AutoSavingJsonRepository<ItemEntity>(fileName);
}
