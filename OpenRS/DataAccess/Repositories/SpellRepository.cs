using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class SpellRepository(string fileName) : JsonRepository<SpellEntity>(fileName)
    {
        public override void Update(SpellEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
