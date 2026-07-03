using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Quest repository implementation.
    /// </summary>
    public class QuestRepository : XmlRepository<QuestEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public QuestRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified quest.
        /// </summary>
        /// <param name="entity">Quest.</param>
        public override void Update(QuestEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
