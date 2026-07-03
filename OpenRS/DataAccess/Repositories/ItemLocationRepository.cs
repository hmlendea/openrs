using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// ItemLocation repository implementation.
    /// </summary>
    public class ItemLocationRepository : XmlRepository<ItemLocationEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLocationRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public ItemLocationRepository(string fileName)
            : base(fileName)
        {

        }

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
