using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Npc repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="NpcRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class NpcRepository(string fileName) : XmlRepository<NpcEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified npc.
        /// </summary>
        /// <param name="entity">Npc.</param>
        public override void Update(NpcEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
