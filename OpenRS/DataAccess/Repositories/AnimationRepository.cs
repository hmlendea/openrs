using System.Collections.Generic;
using System.IO;
using System.Linq;

using NuciDAL.Repositories;

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
            base.Update(entity);
            SaveChanges();
        }
    }
}
