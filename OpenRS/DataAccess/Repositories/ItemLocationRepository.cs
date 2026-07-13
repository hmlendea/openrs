using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class ItemLocationRepository(string fileName) : XmlRepository<ItemLocationEntity>(fileName)
    {
        public override void Update(ItemLocationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
