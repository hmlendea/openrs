using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// NpcLocation repository implementation.
    /// </summary>
    public class NpcLocationRepository : XmlRepository<NpcLocationEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NpcLocationRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public NpcLocationRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified npcLocation.
        /// </summary>
        /// <param name="entity">NpcLocation.</param>
        public override void Update(NpcLocationEntity entity)
        {
            LoadEntitiesIfNeeded();

            NpcLocationEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(NpcLocationEntity).Replace("Entity", ""));
            }

            entityToUpdate.InitialX = entity.InitialX;
            entityToUpdate.InitialY = entity.InitialY;
            entityToUpdate.MinX = entity.MinX;
            entityToUpdate.MinY = entity.MinY;
            entityToUpdate.MaxX = entity.MaxX;
            entityToUpdate.MaxY = entity.MaxY;

            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
