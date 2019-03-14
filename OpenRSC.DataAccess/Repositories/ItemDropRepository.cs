using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// ItemDrop repository implementation.
    /// </summary>
    public class ItemDropRepository : XmlRepository<ItemDropEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemDropRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public ItemDropRepository(string fileName)
            : base(fileName)
        {
            
        }

        /// <summary>
        /// Updates the specified itemDrop.
        /// </summary>
        /// <param name="entity">ItemDrop.</param>
        public override void Update(ItemDropEntity entity)
        {
            LoadEntitiesIfNeeded();

            ItemDropEntity itemDropEntityToUpdate = Get(entity.Id);

            if (itemDropEntityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(ItemDropEntity));
            }

            itemDropEntityToUpdate.ItemId = entity.ItemId;
            itemDropEntityToUpdate.Amount = entity.Amount;
            itemDropEntityToUpdate.Weight = entity.Weight;
            
            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
