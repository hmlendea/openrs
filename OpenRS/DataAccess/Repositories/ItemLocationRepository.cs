using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// ItemLocation repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ItemLocationRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class ItemLocationRepository(string fileName) : XmlRepository<ItemLocationEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified itemLocation.
        /// </summary>
        /// <param name="entity">ItemLocation.</param>
        public override void Update(ItemLocationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
