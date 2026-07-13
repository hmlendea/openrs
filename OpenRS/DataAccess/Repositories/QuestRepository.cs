using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class QuestRepository(string fileName) : JsonRepository<QuestEntity>(fileName)
    {
        public override void Update(QuestEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
