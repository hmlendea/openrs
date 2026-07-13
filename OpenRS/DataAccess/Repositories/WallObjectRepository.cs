using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class WallObjectRepository(string fileName) : JsonRepository<WallObjectEntity>(fileName)
    {
        public override void Update(WallObjectEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
