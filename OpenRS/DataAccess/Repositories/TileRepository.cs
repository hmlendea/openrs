using NuciDAL.Repositories;

using OpenRS.DataAccess.DataObjects;

namespace OpenRS.DataAccess.Repositories
{
    /// <summary>
    /// Tile repository implementation.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="TileRepository"/> class.
    /// </remarks>
    /// <param name="fileName">File name.</param>
    public sealed class TileRepository(string fileName) : XmlRepository<TileEntity>(fileName)
    {
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
