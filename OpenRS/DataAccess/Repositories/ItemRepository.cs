using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class ItemRepository(string fileName) : XmlRepository<ItemEntity>(fileName)
    {
        public override void Update(ItemEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
