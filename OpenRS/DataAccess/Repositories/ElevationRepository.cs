using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class ElevationRepository(string fileName) : XmlRepository<ElevationEntity>(fileName)
    {
        public override void Update(ElevationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
