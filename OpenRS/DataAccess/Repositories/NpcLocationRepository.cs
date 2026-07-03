using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
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
            base.Update(entity);
            SaveChanges();
        }
    }
}
