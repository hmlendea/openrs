using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciXNA.DataAccess.Repositories;

using OpenRSC.DataAccess.DataObjects;

namespace OpenRSC.DataAccess.Repositories
{
    /// <summary>
    /// Tile repository implementation.
    /// </summary>
    public class TileRepository : XmlRepository<TileEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TileRepository"/> class.
        /// </summary>
        /// <param name="fileName">File name.</param>
        public TileRepository(string fileName)
            : base(fileName)
        {
            
        }

        /// <summary>
        /// Updates the specified tile.
        /// </summary>
        /// <param name="tileEntity">Tile.</param>
        public override void Update(TileEntity tileEntity)
        {
            LoadEntitiesIfNeeded();

            TileEntity tileEntityToUpdate = Get(tileEntity.Id);

            if (tileEntityToUpdate == null)
            {
                throw new EntityNotFoundException(tileEntity.Id, nameof(TileEntity).Replace("Entity", ""));
            }

            tileEntityToUpdate.Colour = tileEntity.Colour;
            tileEntityToUpdate.Unknown = tileEntity.Unknown;
            tileEntityToUpdate.Type = tileEntity.Type;
            
            XmlFile.SaveEntities(Entities.Values);
        }
    }
}
