using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Spell repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="SpellRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class SpellRepository(string fileName) : XmlRepository<SpellEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified spell.
        /// </summary>
        /// <param name="entity">Spell.</param>
        public override void Update(SpellEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
