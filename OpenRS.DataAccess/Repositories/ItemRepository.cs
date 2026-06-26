using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Item repository implementation.
    /// </summary>
    public class ItemRepository : XmlRepository<ItemEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public ItemRepository(string fileName)
            : base(fileName)
        {

        }

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
