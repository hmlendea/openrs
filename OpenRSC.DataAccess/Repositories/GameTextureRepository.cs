using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// Texture repository implementation.
    /// </summary>
    public class GameTextureRepository : XmlRepository<GameTextureEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameTextureRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public GameTextureRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified texture.
        /// </summary>
        /// <param name="entity">Texture.</param>
        public override void Update(GameTextureEntity entity)
        {
            LoadEntitiesIfNeeded();

            GameTextureEntity textureEntityToUpdate = Get(entity.Id);

            if (textureEntityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(GameTextureEntity));
            }

            textureEntityToUpdate.Name = entity.Name;
            textureEntityToUpdate.SubName = entity.SubName;
            
            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
