﻿using NuciXNA.DataAccess.DataObjects;

namespace OpenRS.DataAccess.DataObjects
{
    /// <summary>
    /// Texture entity.
    /// </summary>
    public class GameTextureEntity : EntityBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sub-name.
        /// </summary>
        /// <value>The sub-name.</value>
        public string SubName { get; set; }
    }
}
