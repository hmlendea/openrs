using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

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
            LoadEntitiesIfNeeded();

            WorldObjectEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(WorldObjectEntity));
            }

            entityToUpdate.Name = entity.Name;
            entityToUpdate.Description = entity.Description;
            entityToUpdate.Command1 = entity.Command1;
            entityToUpdate.Command2 = entity.Command2;
            entityToUpdate.Type = entity.Type;
            entityToUpdate.Width = entity.Width;
            entityToUpdate.Height = entity.Height;
            entityToUpdate.GroundItemVar = entity.GroundItemVar;
            entityToUpdate.ObjectModel = entity.ObjectModel;
            entityToUpdate.ModelId = entity.ModelId;
            
            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
