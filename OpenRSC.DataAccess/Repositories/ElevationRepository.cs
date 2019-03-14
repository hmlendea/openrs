using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
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
            LoadEntitiesIfNeeded();

            ElevationEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(ElevationEntity));
            }

            entityToUpdate.Roof = entity.Roof;
            entityToUpdate.Unknown = entity.Unknown;

            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
