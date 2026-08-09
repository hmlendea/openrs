using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class TileRepository(string fileName)
        : AutoSavingJsonRepository<TileEntity>(fileName);
}
