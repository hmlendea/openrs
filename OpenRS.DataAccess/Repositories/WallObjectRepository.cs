using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// WallObject repository implementation.
    /// </summary>
    public class WallObjectRepository : XmlRepository<WallObjectEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WallObjectRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public WallObjectRepository(string fileName)
            : base(fileName)
        {

        }

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
