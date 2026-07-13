using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class WorldObjectRepository(string fileName) : JsonRepository<WorldObjectEntity>(fileName)
    {
        public override void Update(WorldObjectEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
