using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Animation repository implementation.
    /// </summary>
    public class AnimationRepository : XmlRepository<AnimationEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public AnimationRepository(string fileName)
            : base(fileName)
        {
        }

        /// <summary>
        /// Updates the specified animation.
        /// </summary>
        /// <param name="entity">Animation.</param>
        public override void Update(AnimationEntity entity)
        {
            LoadEntitiesIfNeeded();

            AnimationEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(AnimationEntity));
            }

            entityToUpdate.Name = entity.Name;
            entityToUpdate.CharacterColour = entity.CharacterColour;
            entityToUpdate.GenderModel = entity.GenderModel;
            entityToUpdate.HasA = entity.HasA;
            entityToUpdate.HasF = entity.HasF;
            entityToUpdate.Number = entity.Number;

            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
