using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Npc repository implementation.
    /// </summary>
    public class NpcRepository : XmlRepository<NpcEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NpcRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public NpcRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified npc.
        /// </summary>
        /// <param name="entity">Npc.</param>
        public override void Update(NpcEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
