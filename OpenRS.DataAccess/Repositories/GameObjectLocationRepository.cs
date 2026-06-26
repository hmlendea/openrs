using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
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
            base.Update(entity);
            SaveChanges();
        }
    }
}
