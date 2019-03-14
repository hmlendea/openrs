using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// Item repository implementation.
    /// </summary>
    public class ItemRepository : XmlRepository<ItemEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public ItemRepository(string fileName)
            : base(fileName)
        {

        }

        /// <summary>
        /// Updates the specified item.
        /// </summary>
        /// <param name="entity">Item.</param>
        public override void Update(ItemEntity entity)
        {
            LoadEntitiesIfNeeded();

            ItemEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(ItemEntity));
            }

            entityToUpdate.Description = entity.Description;
            entityToUpdate.Command = entity.Command;
            entityToUpdate.BasePrice = entity.BasePrice;
            entityToUpdate.SpriteId = entity.SpriteId;
            entityToUpdate.InventoryPicture = entity.InventoryPicture;
            entityToUpdate.PictureMask = entity.PictureMask;
            entityToUpdate.IsEquipable = entity.IsEquipable;
            entityToUpdate.IsPremium = entity.IsPremium;
            entityToUpdate.IsSpecial = entity.IsSpecial;
            entityToUpdate.IsStackable = entity.IsStackable;
            entityToUpdate.IsUnused = entity.IsUnused;

            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
