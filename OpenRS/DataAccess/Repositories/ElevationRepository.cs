using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Elevation repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ElevationRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class ElevationRepository(string fileName) : XmlRepository<ElevationEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified elevation.
        /// </summary>
        /// <param name="entity">Elevation.</param>
        public override void Update(ElevationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
