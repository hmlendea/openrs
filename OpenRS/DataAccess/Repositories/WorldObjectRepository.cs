using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// worldObject repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="WorldObjectRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class WorldObjectRepository(string fileName) : XmlRepository<WorldObjectEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified world object.
        /// </summary>
        /// <param name="entity">World object.</param>
        public override void Update(WorldObjectEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
