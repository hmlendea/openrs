using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// ItemDrop repository implementation.
    /// </summary>
    public class ItemDropRepository : XmlRepository<ItemDropEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDropRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public ItemDropRepository(string fileName)
            : base(fileName)
        {

        }

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
