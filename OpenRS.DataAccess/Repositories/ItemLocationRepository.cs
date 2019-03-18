using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// ItemLocation repository implementation.
    /// </summary>
    public class ItemLocationRepository : XmlRepository<ItemLocationEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemLocationRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public ItemLocationRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified itemLocation.
        /// </summary>
        /// <param name="entity">ItemLocation.</param>
        public override void Update(ItemLocationEntity entity)
        {
            LoadEntitiesIfNeeded();

            ItemLocationEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(ItemLocationEntity));
            }

            entityToUpdate.X = entity.X;
            entityToUpdate.Y = entity.Y;
            entityToUpdate.Amount = entity.Amount;
            entityToUpdate.RespawnTime = entity.RespawnTime;

            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
