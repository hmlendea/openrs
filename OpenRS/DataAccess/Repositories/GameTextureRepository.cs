using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Texture repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GameTextureRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class GameTextureRepository(string fileName) : XmlRepository<GameTextureEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified texture.
        /// </summary>
        /// <param name="entity">Texture.</param>
        public override void Update(GameTextureEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
