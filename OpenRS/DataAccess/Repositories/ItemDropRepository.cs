using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class ItemDropRepository(string fileName) : XmlRepository<ItemDropEntity>(fileName)
    {
        public override void Update(ItemDropEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
