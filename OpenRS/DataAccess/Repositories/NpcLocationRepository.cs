using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class NpcLocationRepository(string fileName) : XmlRepository<NpcLocationEntity>(fileName)
    {
        public override void Update(NpcLocationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
