using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// WallObject repository implementation.
    /// </summary>
    public class WallObjectRepository : XmlRepository<WallObjectEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WallObjectRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public WallObjectRepository(string fileName)
            : base(fileName)
        {
            
        }

        /// <summary>
        /// Updates the specified wallObject.
        /// </summary>
        /// <param name="entity">WallObject.</param>
        public override void Update(WallObjectEntity entity)
        {
            LoadEntitiesIfNeeded();

            WallObjectEntity entityToUpdate = Get(entity.Id);

            if (entityToUpdate == null)
            {
                throw new EntityNotFoundException(entity.Id, nameof(WallObjectEntity));
            }

            entityToUpdate.Name = entity.Name;
            entityToUpdate.Description = entity.Description;
            entityToUpdate.Command1 = entity.Command1;
            entityToUpdate.Command2 = entity.Command2;
            entityToUpdate.Type = entity.Type;
            entityToUpdate.Unknown = entity.Unknown;
            entityToUpdate.ModelHeight = entity.ModelHeight;
            entityToUpdate.ModelFaceBack = entity.ModelFaceBack;
            entityToUpdate.ModelFaceFront = entity.ModelFaceFront;
            
            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
