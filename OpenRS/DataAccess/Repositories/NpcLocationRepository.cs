using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// NpcLocation repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NpcLocationRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class NpcLocationRepository(string fileName) : XmlRepository<NpcLocationEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified npcLocation.
        /// </summary>
        /// <param name="entity">NpcLocation.</param>
        public override void Update(NpcLocationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
