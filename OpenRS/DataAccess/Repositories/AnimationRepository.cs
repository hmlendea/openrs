using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class AnimationRepository(string fileName) : XmlRepository<AnimationEntity>(fileName)
    {
        public override void Update(AnimationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
