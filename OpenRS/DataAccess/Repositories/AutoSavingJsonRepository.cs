using NuciDAL.DataObjects;
using NuciDAL.Repositories;

namespace OpenRS.DataAccess.Repositories
{
    public abstract class AutoSavingJsonRepository<TEntity>(string fileName)
        : JsonRepository<TEntity>(fileName)
        where TEntity : EntityBase
    {
        public override void Update(TEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
