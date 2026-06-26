using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Elevation repository implementation.
    /// </summary>
    public class ElevationRepository : XmlRepository<ElevationEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElevationRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public ElevationRepository(string fileName)
            : base(fileName)
        {
        }

        /// <summary>
        /// Updates the specified elevation.
        /// </summary>
        /// <param name="entity">Elevation.</param>
        public override void Update(ElevationEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
