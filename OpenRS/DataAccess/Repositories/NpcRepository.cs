using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class NpcRepository(string fileName) : JsonRepository<NpcEntity>(fileName)
    {
        public override void Update(NpcEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
