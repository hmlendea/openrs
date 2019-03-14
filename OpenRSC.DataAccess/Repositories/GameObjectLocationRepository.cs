using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// gameObjectLocation repository implementation.
    /// </summary>
    public class GameObjectLocationRepository : XmlRepository<GameObjectLocationEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameObjectLocationRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public GameObjectLocationRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified world object.
        /// </summary>
        /// <param name="entity">World object.</param>
        public override void Update(GameObjectLocationEntity entity)
        {
            LoadEntitiesIfNeeded();

            GameObjectLocationEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(GameObjectLocationEntity));
            }

            entityToUpdate.X = entity.X;
            entityToUpdate.Y = entity.Y;
            entityToUpdate.Direction = entity.Type;
            entityToUpdate.Type = entity.Type;
            
            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
