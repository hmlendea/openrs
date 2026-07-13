using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class GameTextureRepository(string fileName) : XmlRepository<GameTextureEntity>(fileName)
    {
        public override void Update(GameTextureEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
