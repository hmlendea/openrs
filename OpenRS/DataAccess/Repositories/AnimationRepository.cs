using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    public sealed class AnimationRepository(string fileName)
        : AutoSavingJsonRepository<AnimationEntity>(fileName);
}

