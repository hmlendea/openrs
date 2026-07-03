using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// WallObject repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="WallObjectRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class WallObjectRepository(string fileName) : XmlRepository<WallObjectEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified wallObject.
        /// </summary>
        /// <param name="entity">WallObject.</param>
        public override void Update(WallObjectEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
