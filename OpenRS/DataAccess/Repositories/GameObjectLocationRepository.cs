using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class GameObjectLocationRepository(string fileName) : XmlRepository<GameObjectLocationEntity>(fileName)
    {
        public override void Update(GameObjectLocationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
