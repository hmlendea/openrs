using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Spell repository implementation.
    /// </summary>
    public class SpellRepository : XmlRepository<SpellEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpellRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public SpellRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified spell.
        /// </summary>
        /// <param name="entity">Spell.</param>
        public override void Update(SpellEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
