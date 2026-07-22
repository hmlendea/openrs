using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class GameTextureRepository(string fileName)
        : AutoSavingJsonRepository<GameTextureEntity>(fileName);
}
