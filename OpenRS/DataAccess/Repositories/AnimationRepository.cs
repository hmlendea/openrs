using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Animation repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="AnimationRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class AnimationRepository(string fileName) : XmlRepository<AnimationEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified animation.
        /// </summary>
        /// <param name="entity">Animation.</param>
        public override void Update(AnimationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
