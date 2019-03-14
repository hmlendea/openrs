using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
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
            LoadEntitiesIfNeeded();

            SpellEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(SpellEntity));
            }

            entityToUpdate.Name = entity.Name;
            entityToUpdate.Description = entity.Description;
            entityToUpdate.RequiredLevel = entity.RequiredLevel;
            entityToUpdate.Type = entity.Type;
            entityToUpdate.RuneCount = entity.RuneCount;
            entityToUpdate.RequiredRunesIds = entity.RequiredRunesIds;
            entityToUpdate.RequiredRunesCounts = entity.RequiredRunesCounts;
            entityToUpdate.ExperienceGain = entity.ExperienceGain;
            
            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
