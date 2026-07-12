using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// ItemDrop repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="ItemDropRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class ItemDropRepository(string fileName) : XmlRepository<ItemDropEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified itemDrop.
        /// </summary>
        /// <param name="entity">ItemDrop.</param>
        public override void Update(ItemDropEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
