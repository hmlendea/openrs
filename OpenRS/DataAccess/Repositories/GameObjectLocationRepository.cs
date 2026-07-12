using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// gameObjectLocation repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="GameObjectLocationRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class GameObjectLocationRepository(string fileName) : XmlRepository<GameObjectLocationEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified world object.
        /// </summary>
        /// <param name="entity">World object.</param>
        public override void Update(GameObjectLocationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
