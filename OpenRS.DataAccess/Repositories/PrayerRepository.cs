using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Prayer repository implementation.
    /// </summary>
    public class PrayerRepository : XmlRepository<PrayerEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrayerRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public PrayerRepository(string fileName)
            : base(fileName)
        {
            
        }

        /// <summary>
        /// Updates the specified prayer.
        /// </summary>
        /// <param name="entity">Prayer.</param>
        public override void Update(PrayerEntity entity)
        {
            LoadEntitiesIfNeeded();

            PrayerEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(PrayerEntity));
            }

            entityToUpdate.Name = entity.Name;
            entityToUpdate.Description = entity.Description;
            entityToUpdate.RequiredLevel = entity.RequiredLevel;
            entityToUpdate.DrainRate = entity.DrainRate;

            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
