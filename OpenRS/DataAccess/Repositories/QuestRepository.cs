using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class QuestRepository(string fileName)
        : AutoSavingJsonRepository<QuestEntity>(fileName);
}
