using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Prayer repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="PrayerRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public class PrayerRepository(string fileName) : XmlRepository<PrayerEntity>(fileName)
    {
        /// <summary>
        /// Updates the specified prayer.
        /// </summary>
        /// <param name="entity">Prayer.</param>
        public override void Update(PrayerEntity entity)
        {
            base.Update(entity);
            SaveChanges();
        }
    }
}
