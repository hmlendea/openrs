using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
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
            base.Update(entity);
            SaveChanges();
        }
    }
}
