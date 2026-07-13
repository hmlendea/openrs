using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class TileRepository(string fileName) : XmlRepository<TileEntity>(fileName)
    {
        public override void Update(TileEntity tileEntity)
        {
            base.Update(tileEntity);
            SaveChanges();
        }
    }
}
