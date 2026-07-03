using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Item repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ItemRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class ItemRepository(string fileName) : XmlRepository<ItemEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="entity">Item.</param>
        public override void Update(ItemEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
