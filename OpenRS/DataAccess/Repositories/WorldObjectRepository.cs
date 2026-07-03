using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// worldObject repository implementation.
    /// </summary>
    public class WorldObjectRepository : XmlRepository<WorldObjectEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorldObjectRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public WorldObjectRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified world object.
        /// </summary>
        /// <param name="entity">World object.</param>
        public override void Update(WorldObjectEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
