using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
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
            base.Update(tileEntity);
            SaveChanges();
        }
    }
}
