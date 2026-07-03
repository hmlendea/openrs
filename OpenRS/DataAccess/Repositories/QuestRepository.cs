using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Quest repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="QuestRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class QuestRepository(string fileName) : XmlRepository<QuestEntity>(fileName)
    {
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
